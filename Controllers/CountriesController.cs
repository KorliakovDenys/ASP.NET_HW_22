using ASP.NET_HW_22.Models;
using ASP.NET_HW_22.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;

namespace ASP.NET_HW_22.Controllers;

public class CountriesController : Controller {
    private readonly IRepository<Continent> _continentRepository;
    private readonly IRepository<Country> _countryRepository;
    private readonly IRepository<City> _cityRepository;

    public CountriesController(IRepository<Continent> continentRepository, IRepository<Country> countryRepository,
        IRepository<City> cityRepository) {
        _continentRepository = continentRepository;
        _countryRepository = countryRepository;
        _cityRepository = cityRepository;
    }

    public async Task<IActionResult> Index() {
        var result = await _countryRepository.SelectAsync();

        var countries = result.ToList();
        foreach (var country in countries) {
            if (country is null) continue;
            if (country.CapitalObjectId is not null)
                country.Capital = await _cityRepository.SelectAsync((ObjectId)country.CapitalObjectId);
            if (country.ContinentObjectId is not null)
                country.Continent = await _continentRepository.SelectAsync((ObjectId)country.ContinentObjectId);
        }

        return View(countries.ToList());
    }

    public async Task<IActionResult> Details(string? id) {
        if (id == null) {
            return NotFound();
        }

        var country = await GetFullInfoAsync(id);

        if (country is null) {
            return NotFound();
        }

        return View(country);
    }

    public async Task<IActionResult> Create() {
        var continents = await _continentRepository.SelectAsync().ConfigureAwait(false);
        var cities = await _cityRepository.SelectAsync().ConfigureAwait(false);

        ViewData["CapitalObjectId"] = new SelectList(cities, "Id", "Name");
        ViewData["ContinentObjectId"] = new SelectList(continents, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Name, Population, Area, CapitalObjectId, ContinentObjectId")]
        Country country) {
        if (!ModelState.IsValid) return View(country);
        await _countryRepository.InsertAsync(country);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string? id) {
        if (id == null) {
            return NotFound();
        }

        var country = await _countryRepository.SelectAsync(ObjectId.Parse(id));
        if (country == null) {
            return NotFound();
        }

        var continents = await _continentRepository.SelectAsync().ConfigureAwait(false);
        var cities = await _cityRepository.SelectAsync().ConfigureAwait(false);

        ViewData["CapitalObjectId"] = new SelectList(cities, "Id", "Name");
        ViewData["ContinentObjectId"] = new SelectList(continents, "Id", "Name");

        return View(country);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id,
        [Bind("Id, Name, Area, Population, CapitalObjectId, ContinentObjectId")]
        Country country) {
        if (ObjectId.Parse(id) != country.Id) {
            return NotFound();
        }

        if (ModelState.IsValid) {
            try {
                await _countryRepository.UpdateAsync(ObjectId.Parse(id), country);
            }
            catch (Exception) {
                if (!CountryExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        return View(country);
    }

    public async Task<IActionResult> Delete(string? id) {
        if (id == null) {
            return NotFound();
        }

        var country = await GetFullInfoAsync(id);

        if (country == null) {
            return NotFound();
        }

        return View(country);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id) {
        var country = await _countryRepository.SelectAsync(ObjectId.Parse(id));
        if (country != null) {
            await _countryRepository.DeleteAsync(country.Id);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<Country?> GetFullInfoAsync(string id) {
        var country = await _countryRepository.SelectAsync(ObjectId.Parse(id));
        if (country is null)
            return null;

        if (country.CapitalObjectId is not null)
            country.Capital = await _cityRepository.SelectAsync((ObjectId)country.CapitalObjectId);
        if (country.ContinentObjectId is not null)
            country.Continent = await _continentRepository.SelectAsync((ObjectId)country.ContinentObjectId);
        
        return country;
    }

    private bool CountryExists(string id) {
        return _countryRepository.AnyAsync(ObjectId.Parse(id)).Result;
    }
}