using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{

    [ApiController]
    [Route("/api/auctions")]
    public class AuctionController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;
        public AuctionController(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDTO>>> GetAllAuctions()
        {
            var auctions = await _context.Auctions.Include(x => x.Item).ToListAsync();
            return _mapper.Map<List<AuctionDTO>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDTO>> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if (auction is null) return NotFound();

            return _mapper.Map<AuctionDTO>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDTO>> PostAuction(CreateAuctionDTO dto)
        {
            var auction = _mapper.Map<Auction>(dto);
            //TODO: add current user as Seller
            auction.Seller = "Raendy";

            _context.Auctions.Add(auction);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not save changes to the database");

            return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, _mapper.Map<AuctionDTO>(auction));

        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AuctionDTO>> UpdateAuction(Guid id, UpdateAuctionDTO dto)
        {
            var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if (auction is null) return NotFound();

            //TODO: check current seller == username

            auction.Item.Make = dto.Make ?? auction.Item.Make;
            auction.Item.Model = dto.Model ?? auction.Item.Model;
            auction.Item.Color = dto.Color ?? auction.Item.Color;
            auction.Item.Mileage = dto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = dto.Year ?? auction.Item.Year;

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("Could not save changes to the database");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<AuctionDTO>> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);

            if (auction is null) return NotFound();

            //TODO: check current seller == username

            _context.Auctions.Remove(auction);

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not save changes to the database");

            return Ok();
        }

    }
}
