using HotelManager.Core;
using HotelManager.Data;
using HotelManager.DTO;
using HotelManager.Service.Contract;
using HotelManager.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : Controller
    {
        public readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        public RoomController(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [Route("")]
        public IActionResult Get()
        {
            try
            {
                var rooms = _context.Room.Where(r => !r.IsDeleted)
                    .Select(r => new RoomModel
                    {
                        Id = r.Id,
                        NextAvailableDateString = r.NextAvailableDate.ToString("dd/MMM/yyyy - hh:mm"),
                        Size = r.Size,
                        Status = IsRoomAvailable(r.NextAvailableDate) ? "Available" : "Booked",
                    });
                //.Join(_context.Users, room => room.CurrentGuest, user => user.Id,
                //(room, user) => new RoomModel
                //{
                //    Id = room.Id,
                //    DateCreated = room.DateCreated,
                //    CurrentGuest = IsRoomAvailable(room.NextAvailableDate) ? "" : room.CurrentGuest,
                //    NextAvailableDate = room.NextAvailableDate
                //});

                return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status200OK,
                    data = rooms,
                    serverError = false,
                    validationError = false,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "Internal server error",
                    serverError = true
                });
            }
        }

        [Route("available")]
        public IActionResult GetAvailableRooms()
        {
            try
            {
                var rooms = _context.Room.Where(r => !r.IsDeleted && r.NextAvailableDate < DateTime.Now)
                    .Select(r=>new RoomModel { 
                        Id = r.Id,
                        NextAvailableDateString = r.NextAvailableDate.ToString("dd/MMM/yyyy - hh:mm"),
                        Size = r.Size
                    });;

                return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status200OK,
                    data = rooms,
                    serverError = false,
                    validationError = false,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "Internal server error",
                    serverError = true
                });
            }
        }


        [HttpPost]
        [Route("create-new-room")]
        [Authorize(Roles =AppConstant.SuperAdminRole)]
        public async Task<IActionResult> Post([FromBody] RoomModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                    {
                        statusCode = StatusCodes.Status400BadRequest,
                        message = "Fill all required fields",
                        serverError = false,
                        validationError = true
                    }) ;
                }

                var room = new Room
                {
                    DateCreated = DateTime.Now,
                    CurrentGuest = "",
                    IsDeleted = false,
                    NextAvailableDate = DateTime.Now,
                    Size = model.Size
                };
                _context.Room.Add(room);
                await _context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status200OK,
                    data = room,
                    serverError = false,
                    validationError = false
                }) ;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "Internal server error",
                    serverError = true,
                });
            }
        }

        [Route("update-room")]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] RoomModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                    {
                        statusCode = StatusCodes.Status400BadRequest,
                        message = "Fill all required fields",
                        serverError = false,
                        validationError = true,
                    });
                }

                var room = _context.Room.Find(model.Id);
                if (room == null)
                    return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Room not found",
                        serverError = false,
                        validationError = true,
                    });

                //room.IsAvailable = model.IsAvailable;
                //room.CurrentGuest = model.CurrentGuest;
                room.NextAvailableDate = DateTime.Now.AddDays(31);

                _context.Entry(room).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status200OK,
                    data = room,
                    serverError = false,
                    validationError = false
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "Internal server error",
                    serverError = true
                });
            }
        }

        [Route("cancel-booking/{id}")]
        [Authorize(Roles =AppConstant.GuestUserRole)]
        public async Task<IActionResult> CancelBooking(int id)
        {
            try
            {
                var room = _context.Room.Find(id);
                if (room == null)
                    return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Room not found",
                        serverError = false,
                        validationError = true
                    });

                var userId = _jwtService.GetLoggedInUserId();
                if (!(string.Compare(room.CurrentGuest, userId, false) == 0))
                {
                    return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                    {
                        statusCode = StatusCodes.Status400BadRequest,
                        message = "You do not have access to this room",
                        serverError = false,
                        validationError = true
                    });
                }

                room.CurrentGuest = "";
                room.NextAvailableDate = DateTime.Now;

                _context.Entry(room).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status200OK,
                    data = "",
                    serverError = false,
                    validationError = false,
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "Internal server error",
                    serverError = true
                });
            }
        }

        [Route("book/{id}")]
        [Authorize(Roles =AppConstant.GuestUserRole)]
        public async Task<IActionResult> BookRoom(int id, int days)
        {
            try
            {
                var room = _context.Room.Find(id);
                if (room == null)
                    return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = "Room not found",
                        serverError = false,
                        validationError = true
                    });

                var userId = _jwtService.GetLoggedInUserId();
                if (!IsRoomAvailable(room.NextAvailableDate))
                    return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                    {
                        statusCode = StatusCodes.Status200OK,
                        message = "Room not available",
                        serverError = false,
                        validationError = true
                    });

                room.NextAvailableDate = DateTime.Now.AddDays(days);
                room.CurrentGuest = userId;

                _context.Entry(room).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status200OK,
                    data = "",
                    serverError = false,
                    validationError = false,
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponseModel
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "Internal server error",
                    serverError = true
                });
            }
        }

        private static bool IsRoomAvailable(DateTime nextAvailableDate)
        {
            return nextAvailableDate < DateTime.Now ? true : false;
        }
    }
}
