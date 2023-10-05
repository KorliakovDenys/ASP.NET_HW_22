using ASP.NET_HW_22.Models;
using ASP.NET_HW_22.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;

namespace ASP.NET_HW_22.Controllers;

public class CitiesController : Controller {
    private readonly IRepository<City> _cityRepository;
    private readonly IRepository<Country> _countryRepository;

    public CitiesController(IRepository<City> cityRepository, IRepository<Country> countryRepository) {
        _cityRepository = cityRepository;
        _countryRepository = countryRepository;
    }

    public async Task<IActionResult> Index() {
        var cities = await _cityRepository.SelectAsync().ConfigureAwait(false);

        return View(cities);
    }

    public async Task<IActionResult> Details(string? id) {
        if (id == null) {
            return NotFound();
        }

        var city = await GetFullInfoAsync(id);

        if (city == null) {
            return NotFound();
        }

        return View(city);
    }

    public async Task<IActionResult> Create() {
        var countries = await _countryRepository.SelectAsync().ConfigureAwait(false);

        ViewData["CountryId"] = new SelectList(countries, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,CountryId")] City city) {
        if (ModelState.IsValid) {
            await _cityRepository.InsertAsync(city);
            return RedirectToAction(nameof(Index));
        }

        var countries = await _countryRepository.SelectAsync().ConfigureAwait(false);

        ViewData["CountryId"] = new SelectList(countries, "Id", "Name");
        return View(city);
    }

    public async Task<IActionResult> Edit(string? id) {
        if (id == null) {
            return NotFound();
        }

        var city = await _cityRepository.SelectAsync(ObjectId.Parse(id));
        if (city == null) {
            return NotFound();
        }

        var countries = await _countryRepository.SelectAsync().ConfigureAwait(false);

        ViewData["CountryId"] = new SelectList(countries, "Id", "Name");
        return View(city);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id, Name,CountryId")] City city) {
        if (ObjectId.Parse(id) != city.Id) {
            return NotFound();
        }

        if (ModelState.IsValid) {
            try {
                await _cityRepository.UpdateAsync(ObjectId.Parse(id), city);
            }
            catch (Exception) {
                if (!CityExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        var countries = await _countryRepository.SelectAsync().ConfigureAwait(false);

        ViewData["CountryId"] = new SelectList(countries, "Id", "Name");
        return View(city);
    }

    public async Task<IActionResult> Delete(string? id) {
        if (id == null) {
            return NotFound();
        }

        var city = await GetFullInfoAsync(id);

        if (city == null) {
            return NotFound();
        }

        return View(city);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id) {
        var city = await _cityRepository.SelectAsync(ObjectId.Parse(id));
        if (city != null) {
            await _cityRepository.DeleteAsync(city.Id);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<City?> GetFullInfoAsync(string id) {
        var city = await _cityRepository.SelectAsync(ObjectId.Parse(id));
        if (city is null)
            return null;
        if (city.CountryId is not null) 
            city.Country = await _countryRepository.SelectAsync((ObjectId)city.CountryId);
        
        return city;
    }

    private bool CityExists(string id) {
        return _cityRepository.AnyAsync(ObjectId.Parse(id)).Result;
    }
}