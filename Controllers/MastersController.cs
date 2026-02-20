using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TscLoanManagement.TSCDB.Application.DTOs;
using TscLoanManagement.TSCDB.Application.DTOs.PurchaseSourceDocuments;
using TscLoanManagement.TSCDB.Application.Interfaces;
using TscLoanManagement.TSCDB.Core.Domain.Masters;
using TscLoanManagement.TSCDB.Infrastructure.Data.Context;
using static TscLoanManagement.TSCDB.Core.Domain.Location;

namespace TscLoanManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MastersController : ControllerBase
    {
        private readonly IMasterService<EntityType> _entityTypeService;
        private readonly IMasterService<BusinessCategory> _businessCategoryService;
        private readonly IMasterService<BusinessType> _businessTypeService;
        private readonly IMasterService<AddressStatus> _addressStatusService;
        private readonly IMasterService<PersonType> _personTypeService;
        private readonly IMasterService<Designation> _designationService;
        private readonly IBankDetailService _bankDetailService;
        private readonly IPurchaseSourceService _purchaseSourceService;
        private readonly IMasterService<Make> _makeService;
        private readonly IMasterService<ChequeLocation> _chequeLocationService;
        private readonly IPurchaseSourceDocumentService _purchaseSourceDocumentService;
        


        private readonly TSCDbContext _context;

        public MastersController(
            IMasterService<EntityType> entityTypeService,
            IMasterService<BusinessCategory> businessCategoryService,
            IMasterService<BusinessType> businessTypeService,
            IMasterService<AddressStatus> addressStatusService,
            IMasterService<PersonType> personTypeService,
            IMasterService<Designation> designationService,
        IBankDetailService bankDetailService,
            IPurchaseSourceService purchaseSourceService,
            IMasterService<Make> makeService, TSCDbContext context, IPurchaseSourceDocumentService purchaseSourceDocumentService, IMasterService<ChequeLocation> chequeLocationService)
        {
            _entityTypeService = entityTypeService;
            _businessCategoryService = businessCategoryService;
            _businessTypeService = businessTypeService;
            _addressStatusService = addressStatusService;
            _personTypeService = personTypeService;
            _bankDetailService = bankDetailService;
            _purchaseSourceService = purchaseSourceService;
            _makeService = makeService;
            _context = context;
            _purchaseSourceDocumentService = purchaseSourceDocumentService;
            _chequeLocationService = chequeLocationService;
            _designationService = designationService;

        }

        [HttpGet("entity-types")] public async Task<IActionResult> GetEntityTypes() => Ok(await _entityTypeService.GetAllAsync());
        [HttpGet("entity-types/{id}")] public async Task<IActionResult> GetEntityType(int id) => Ok(await _entityTypeService.GetByIdAsync(id));
        [HttpPost("entity-types")] public async Task<IActionResult> CreateEntityType([FromBody] MasterDto dto) => Ok(await _entityTypeService.CreateAsync(dto));
        [HttpPut("entity-types/{id}")] public async Task<IActionResult> UpdateEntityType(int id, [FromBody] MasterDto dto) => Ok(await _entityTypeService.UpdateAsync(id, dto));
        [HttpDelete("entity-types/{id}")] public async Task<IActionResult> DeleteEntityType(int id) => Ok(await _entityTypeService.DeleteAsync(id));

        [HttpGet("business-categories")] public async Task<IActionResult> GetBusinessCategories() => Ok(await _businessCategoryService.GetAllAsync());
        [HttpGet("business-categories/{id}")] public async Task<IActionResult> GetBusinessCategory(int id) => Ok(await _businessCategoryService.GetByIdAsync(id));
        [HttpPost("business-categories")] public async Task<IActionResult> CreateBusinessCategory([FromBody] MasterDto dto) => Ok(await _businessCategoryService.CreateAsync(dto));
        [HttpPut("business-categories/{id}")] public async Task<IActionResult> UpdateBusinessCategory(int id, [FromBody] MasterDto dto) => Ok(await _businessCategoryService.UpdateAsync(id, dto));
        [HttpDelete("business-categories/{id}")] public async Task<IActionResult> DeleteBusinessCategory(int id) => Ok(await _businessCategoryService.DeleteAsync(id));

        [HttpGet("business-types")] public async Task<IActionResult> GetBusinessTypes() => Ok(await _businessTypeService.GetAllAsync());
        [HttpGet("business-types/{id}")] public async Task<IActionResult> GetBusinessType(int id) => Ok(await _businessTypeService.GetByIdAsync(id));
        [HttpPost("business-types")] public async Task<IActionResult> CreateBusinessType([FromBody] MasterDto dto) => Ok(await _businessTypeService.CreateAsync(dto));
        [HttpPut("business-types/{id}")] public async Task<IActionResult> UpdateBusinessType(int id, [FromBody] MasterDto dto) => Ok(await _businessTypeService.UpdateAsync(id, dto));
        [HttpDelete("business-types/{id}")] public async Task<IActionResult> DeleteBusinessType(int id) => Ok(await _businessTypeService.DeleteAsync(id));

        [HttpGet("address-statuses")] public async Task<IActionResult> GetAddressStatuses() => Ok(await _addressStatusService.GetAllAsync());
        [HttpGet("address-statuses/{id}")] public async Task<IActionResult> GetAddressStatus(int id) => Ok(await _addressStatusService.GetByIdAsync(id));
        [HttpPost("address-statuses")] public async Task<IActionResult> CreateAddressStatus([FromBody] MasterDto dto) => Ok(await _addressStatusService.CreateAsync(dto));
        [HttpPut("address-statuses/{id}")] public async Task<IActionResult> UpdateAddressStatus(int id, [FromBody] MasterDto dto) => Ok(await _addressStatusService.UpdateAsync(id, dto));
        [HttpDelete("address-statuses/{id}")] public async Task<IActionResult> DeleteAddressStatus(int id) => Ok(await _addressStatusService.DeleteAsync(id));

        [HttpGet("person-types")] public async Task<IActionResult> GetPersonTypes() => Ok(await _personTypeService.GetAllAsync());
        [HttpGet("person-types/{id}")] public async Task<IActionResult> GetPersonType(int id) => Ok(await _personTypeService.GetByIdAsync(id));
        [HttpPost("person-types")] public async Task<IActionResult> CreatePersonType([FromBody] MasterDto dto) => Ok(await _personTypeService.CreateAsync(dto));
        [HttpPut("person-types/{id}")] public async Task<IActionResult> UpdatePersonType(int id, [FromBody] MasterDto dto) => Ok(await _personTypeService.UpdateAsync(id, dto));
        [HttpDelete("person-types/{id}")] public async Task<IActionResult> DeletePersonType(int id) => Ok(await _personTypeService.DeleteAsync(id));

        //[HttpGet("bank-details")] public async Task<IActionResult> GetBankDetails() => Ok(await _bankDetailService.GetAllAsync());
        //[HttpGet("bank-details/{id}")] public async Task<IActionResult> GetBankDetail(int id) => Ok(await _bankDetailService.GetByIdAsync(id));
        //[HttpPost("bank-details")] public async Task<IActionResult> CreateBankDetail([FromBody] MasterDto dto) => Ok(await _bankDetailService.CreateAsync(dto));
        //[HttpPut("bank-details/{id}")] public async Task<IActionResult> UpdateBankDetail(int id, [FromBody] MasterDto dto) => Ok(await _bankDetailService.UpdateAsync(id, dto));
        //[HttpDelete("bank-details/{id}")] public async Task<IActionResult> DeleteBankDetail(int id) => Ok(await _bankDetailService.DeleteAsync(id));
        [HttpGet("bank-details")]
        public async Task<IActionResult> GetBankDetails() => Ok(await _bankDetailService.GetAllAsync());

        [HttpGet("bank-details/{id}")]
        public async Task<IActionResult> GetBankDetail(int id) => Ok(await _bankDetailService.GetByIdAsync(id));

        [HttpPost("bank-details")]
        public async Task<IActionResult> CreateBankDetail([FromBody] BankDetailDto dto) =>
            Ok(await _bankDetailService.CreateAsync(dto));

        [HttpPut("bank-details/{id}")]
        public async Task<IActionResult> UpdateBankDetail(int id, [FromBody] BankDetailDto dto) =>
            Ok(await _bankDetailService.UpdateAsync(id, dto));

        [HttpDelete("bank-details/{id}")]
        public async Task<IActionResult> DeleteBankDetail(int id) =>
            Ok(await _bankDetailService.DeleteAsync(id));
        //[HttpGet("purchase-sources")] public async Task<IActionResult> GetPurchaseSources() => Ok(await _purchaseSourceService.GetAllAsync());
        //[HttpGet("purchase-sources/{id}")] public async Task<IActionResult> GetPurchaseSource(int id) => Ok(await _purchaseSourceService.GetByIdAsync(id));
        //[HttpPost("purchase-sources")] public async Task<IActionResult> CreatePurchaseSource([FromBody] MasterDto dto) => Ok(await _purchaseSourceService.CreateAsync(dto));
        //[HttpPut("purchase-sources/{id}")] public async Task<IActionResult> UpdatePurchaseSource(int id, [FromBody] MasterDto dto) => Ok(await _purchaseSourceService.UpdateAsync(id, dto));
        //[HttpDelete("purchase-sources/{id}")] public async Task<IActionResult> DeletePurchaseSource(int id) => Ok(await _purchaseSourceService.DeleteAsync(id));

        [HttpGet("purchase-sources")]
        public async Task<IActionResult> GetPurchaseSources() =>
    Ok(await _purchaseSourceService.GetAllAsync());

        [HttpGet("purchase-sources/{id}")]
        public async Task<IActionResult> GetPurchaseSource(int id) =>
            Ok(await _purchaseSourceService.GetByIdAsync(id));

        // ✅ FIX: use PurchaseSourceDto instead of MasterDto
        [HttpPost("purchase-sources")]
        public async Task<IActionResult> CreatePurchaseSource([FromBody] PurchaseSourceDto dto) =>
            Ok(await _purchaseSourceService.CreateAsync(dto));

        [HttpPut("purchase-sources/{id}")]
        public async Task<IActionResult> UpdatePurchaseSource(int id, [FromBody] PurchaseSourceDto dto) =>
            Ok(await _purchaseSourceService.UpdateAsync(id, dto));

        [HttpDelete("purchase-sources/{id}")]
        public async Task<IActionResult> DeletePurchaseSource(int id) =>
            Ok(await _purchaseSourceService.DeleteAsync(id));

        //[HttpDelete("purchase-source-documents/{id}")]
        //public async Task<IActionResult> DeletePurchaseSourceDocument(int id) =>
        //    Ok(await _purchaseSourceService.DeleteAsync(id));


        [HttpGet("purchase-sources/{sourceId}/documents")]
        public async Task<IActionResult> GetDocuments(int sourceId) =>
            Ok(await _purchaseSourceDocumentService.GetByPurchaseSourceIdAsync(sourceId));

        [HttpPost("purchase-sources/{sourceId}/documents")]
        public async Task<IActionResult> AddDocuments(int sourceId, [FromBody] PurchaseSourceDocumentCreateManyDto dto)
        {
            dto.PurchaseSourceId = sourceId;
            return Ok(await _purchaseSourceDocumentService.CreateManyAsync(dto));
        }

        [HttpDelete("purchase-source-documents/{id}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await _purchaseSourceDocumentService.DeleteAsync(id));

        [HttpGet("makes")] public async Task<IActionResult> GetMakes() => Ok(await _makeService.GetAllAsync());
        [HttpGet("makes/{id}")] public async Task<IActionResult> GetMake(int id) => Ok(await _makeService.GetByIdAsync(id));
        [HttpPost("makes")] public async Task<IActionResult> CreateMake([FromBody] MasterDto dto) => Ok(await _makeService.CreateAsync(dto));
        [HttpPut("makes/{id}")] public async Task<IActionResult> UpdateMake(int id, [FromBody] MasterDto dto) => Ok(await _makeService.UpdateAsync(id, dto));
        [HttpDelete("makes/{id}")] public async Task<IActionResult> DeleteMake(int id) => Ok(await _makeService.DeleteAsync(id));

        [HttpGet("cheque-location")]
        public async Task<IActionResult> GetChequeLocations() =>
    Ok(await _chequeLocationService.GetAllAsync());

        [HttpGet("cheque-location/{id}")]
        public async Task<IActionResult> GetChequeLocation(int id) =>
            Ok(await _chequeLocationService.GetByIdAsync(id));

        [HttpPost("cheque-location")]
        public async Task<IActionResult> CreateChequeLocation([FromBody] MasterDto dto) =>
            Ok(await _chequeLocationService.CreateAsync(dto));

        [HttpPut("cheque-location/{id}")]
        public async Task<IActionResult> UpdateChequeLocation(int id, [FromBody] MasterDto dto) =>
            Ok(await _chequeLocationService.UpdateAsync(id, dto));

        [HttpDelete("cheque-location/{id}")]
        public async Task<IActionResult> DeleteChequeLocation(int id) =>
            Ok(await _chequeLocationService.DeleteAsync(id));

        [HttpGet("designations")]
        public async Task<IActionResult> GetDesignations() => Ok(await _designationService.GetAllAsync());

        [HttpGet("designations/{id}")]
        public async Task<IActionResult> GetDesignation(int id) => Ok(await _designationService.GetByIdAsync(id));

        [HttpPost("designations")]
        public async Task<IActionResult> CreateDesignation([FromBody] MasterDto dto) =>
            Ok(await _designationService.CreateAsync(dto));

        [HttpPut("designations/{id}")]
        public async Task<IActionResult> UpdateDesignation(int id, [FromBody] MasterDto dto) =>
            Ok(await _designationService.UpdateAsync(id, dto));

        [HttpDelete("designations/{id}")]
        public async Task<IActionResult> DeleteDesignation(int id) =>
            Ok(await _designationService.DeleteAsync(id));


        [HttpPost("states")]
        public async Task<IActionResult> CreateState([FromBody] LocationDto.StateDto stateDto)
        {
            if (string.IsNullOrWhiteSpace(stateDto.Name))
                return BadRequest("State name is required.");

            var state = new State
            {
                Name = stateDto.Name
            };

            _context.States.Add(state);
            await _context.SaveChangesAsync();

            stateDto.Id = state.Id; // return the generated ID

            return CreatedAtAction(nameof(GetStates), new { id = state.Id }, stateDto);
        }

        [HttpPut("states/{id}")]
        public async Task<IActionResult> UpdateState(int id, [FromBody] LocationDto.StateDto stateDto)
        {
            var state = await _context.States.FindAsync(id);
            if (state == null)
                return NotFound("State not found.");

            state.Name = stateDto.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }




        // GET: api/master/states
        [HttpGet("states")]
        public async Task<ActionResult<IEnumerable<LocationDto.StateDto>>> GetStates()
        {
            var states = await _context.States
                .Select(s => new LocationDto.StateDto
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync();

            return Ok(states);
        }

        [HttpPost("cities")]
        public async Task<IActionResult> CreateCity([FromBody] LocationDto.CityDto cityDto)
        {
            if (string.IsNullOrWhiteSpace(cityDto.Name))
                return BadRequest("City name is required.");

            var stateExists = await _context.States.AnyAsync(s => s.Id == cityDto.StateId);
            if (!stateExists)
                return BadRequest("Invalid StateId.");

            var city = new City
            {
                Name = cityDto.Name,
                StateId = cityDto.StateId,
            };

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            cityDto.Id = city.Id;

            return CreatedAtAction(nameof(GetCitiesByState), new { stateId = cityDto.StateId }, cityDto);
        }

        [HttpPut("cities/{id}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] LocationDto.CityDto cityDto)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
                return NotFound("City not found.");

            if (!await _context.States.AnyAsync(s => s.Id == cityDto.StateId))
                return BadRequest("Invalid StateId.");

            city.Name = cityDto.Name;
            city.StateId = cityDto.StateId;

            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpGet("states/{stateId}/cities")]
        public async Task<ActionResult<IEnumerable<LocationDto.CityDto>>> GetCitiesByState(int stateId)
        {
            var cities = await _context.Cities
                .Where(c => c.StateId == stateId)
                .Select(c => new LocationDto.CityDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return Ok(cities);
        }
    }
}
