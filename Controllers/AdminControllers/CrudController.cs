using Code123.Data;
using Code123.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Code123.Controllers;

[AutoValidateAntiforgeryToken]
public abstract class CrudController<T> : Controller where T : class, IEntity, new()
{
    // Store context via a protected property initialized by the constructor
    protected ApplicationDbContext Context { get; }

    protected CrudController(ApplicationDbContext context)
    {
        Context = context;
    }

    protected DbSet<T> DbSet => Context.Set<T>();
    protected abstract string DisplayName { get; }

    // Get the view path based on the derived controller's name.
    private string GetAdminViewPath(string viewName)
    {
        string controllerName = GetType().Name.Replace("Controller", "");

        return $"~/Views/Admin/{controllerName}/{viewName}.cshtml";
    }

    // Extension points
    protected virtual IQueryable<T> GetQuery() => DbSet.AsNoTracking().OrderBy(e => e.Id);
    protected virtual Task PopulateLookupsAsync(T model) => Task.CompletedTask;
    protected virtual Task ValidateAsync(T model, int? id) => Task.CompletedTask;
    protected virtual void OnCreating(T model) { }
    protected abstract void Apply(T existing, T model);
    protected virtual Task<string?> CheckDeleteAsync(T entity) => Task.FromResult<string?>(null);

    // Index
    [HttpGet]
    public virtual async Task<IActionResult> Index()
    {
        var model = await GetQuery().ToListAsync();

        return View(GetAdminViewPath("Index"), model);
    }

    // Create (HttpGet)
    [HttpGet]
    public virtual async Task<IActionResult> Create()
    {
        var path = GetAdminViewPath("Create");

        // Modal-only pages have no Create view, so send them to the list
        if (!ViewExists(path))
            return RedirectToAction(nameof(Index));

        var model = new T();
        await PopulateLookupsAsync(model);
        return View(path, model);
    }

    // Create (HttpPost)
    [HttpPost]
    public virtual async Task<IActionResult> Create(T model)
    {
        await ValidateAsync(model, null);
        if (ModelState.IsValid)
        {
            OnCreating(model);
            DbSet.Add(model);
            if (await TrySaveAsync())
            {
                SetSuccess($"{DisplayName} created successfully.");
                return RedirectToAction(nameof(Index));
            }
        }
        return await InvalidAsync(model, "Create");
    }

    // Edit (HttpGet)
    [HttpGet]
    public virtual async Task<IActionResult> Edit(int id)
    {
        var path = GetAdminViewPath("Edit");

        // Modal-only pages have no Edit view, so send them to the list
        if (!ViewExists(path))
            return RedirectToAction(nameof(Index));

        var entity = await DbSet.FindAsync(id);
        if (entity == null) return NotFound();
        await PopulateLookupsAsync(entity);
        return View(path, entity);
    }

    // Edit (HttpPost)
    [HttpPost]
    public virtual async Task<IActionResult> Edit(int id, T model)
    {
        var existing = await DbSet.FindAsync(id);
        if (existing is null) return NotFound();

        model.Id = id;
        await ValidateAsync(model, id);
        if (ModelState.IsValid)
        {
            Apply(existing, model);
            if (await TrySaveAsync())
            {
                SetSuccess($"{DisplayName} updated successfully.");
                return RedirectToAction(nameof(Index));
            }
        }
        return await InvalidAsync(model, "Edit");
    }

    // Delete
    [HttpPost, ActionName("Delete")]
    public virtual async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity is null) return NotFound();

        if (await CheckDeleteAsync(entity) is { } reason)
        {
            SetError(reason);
            return RedirectToAction(nameof(Index));
        }

        DbSet.Remove(entity);
        if (await TrySaveAsync())
            SetSuccess($"{DisplayName} deleted successfully.");
        else
            SetError($"{DisplayName} could not be deleted.");
        return RedirectToAction(nameof(Index));
    }

    // Helpers

    // Handles a failed Create/Edit. If a full-page view exists, show it with the
    // validation errors. Otherwise (modal-only pages) go back to the list and
    // show the errors in the alert banner.
    private async Task<IActionResult> InvalidAsync(T model, string viewName)
    {
        var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .Distinct()
            .Take(3)
            .ToList();

        SetError(errors.Count > 0
            ? "Please correct the errors: " + string.Join(" ", errors)
            : "Please correct the errors below.");

        var path = GetAdminViewPath(viewName);

        if (!ViewExists(path))
            return RedirectToAction(nameof(Index));

        await PopulateLookupsAsync(model);
        return View(path, model);
    }

    private bool ViewExists(string path)
    {
        var engine = HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>();
        return engine.GetView(executingFilePath: null, viewPath: path, isMainPage: true).Success;
    }

    // Both key styles are set so either alert partial reads them
    private void SetSuccess(string message)
    {
        TempData["SuccessMessage"] = message;
        TempData["Success"] = message;
    }

    private void SetError(string message)
    {
        TempData["ErrorMessage"] = message;
        TempData["Error"] = message;
    }

    private async Task<bool> TrySaveAsync()
    {
        try
        {
            await Context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "The record could not be saved. It may conflict with existing data.");
            return false;
        }
    }
}