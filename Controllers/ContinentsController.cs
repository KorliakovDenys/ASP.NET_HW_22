using ASP.NET_HW_22.Models;
using ASP.NET_HW_22.Repository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ASP.NET_HW_22.Controllers;

public class ContinentsController : Controller {
    private readonly IRepository<Continent> _continentRepository;

    public ContinentsController(IRepository<Continent> continentRepository) {
        _continentRepository = continentRepository;
    }

    public async Task<IActionResult> Index() {
        var continents = await _continentRepository.SelectAsync().ConfigureAwait(false);

        return View(continents);
    }

    public async Task<IActionResult> Details(string? id) {
        if (id == null) {
            return NotFound();
        }

        var continent = await _continentRepository.SelectAsync(ObjectId.Parse(id));

        if (continent == null) {
            return NotFound();
        }

        return View(continent);
    }

    public Task<IActionResult> Create() {
        return Task.FromResult<IActionResult>(View());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name")] Continent continent) {
        if (!ModelState.IsValid) return View(continent);
        await _continentRepository.InsertAsync(continent);
        
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string? id) {
        if (id == null) {
            return NotFound();
        }

        var continent = await _continentRepository.SelectAsync(ObjectId.Parse(id));
        if (continent == null) {
            return NotFound();
        }

        return View(continent);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Name")] Continent continent) {
        if (ObjectId.Parse(id) != continent.Id) {
            return NotFound();
        }

        if (ModelState.IsValid) {
            try {
                await _continentRepository.UpdateAsync(ObjectId.Parse(id), continent);
            }
            catch (Exception) {
                if (!ContinentExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        return View(continent);
    }

    public async Task<IActionResult> Delete(string? id) {
        if (id == null) {
            return NotFound();
        }

        var continent = await _continentRepository.SelectAsync(ObjectId.Parse(id));
        if (continent == null) {
            return NotFound();
        }

        return View(continent);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id) {
        var continent = await _continentRepository.SelectAsync(ObjectId.Parse(id));
        if (continent != null) {
            await _continentRepository.DeleteAsync(continent.Id);
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ContinentExists(string id) {
        return _continentRepository.AnyAsync(ObjectId.Parse(id)).Result;
    }
}