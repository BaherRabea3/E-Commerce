using API.Requests.Addresses;
using Application.Features.Addresses.Commands.CreateAddress;
using Application.Features.Addresses.Commands.DeleteAddress;
using Application.Features.Addresses.Commands.UpdateAddress;
using Application.Features.Addresses.Queries.GetCustomerAddresses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace API.Controllers
{
    public class AddressesController : APIController
    {
        public AddressesController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerAddresses()
        {
            var response = await _mediator.Send(new GetCustomerAddressesQuery(CustomerId));

            return response.IsSuccess ? Ok(response.Value) : HandleFailure(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAddressRequest request)
        {
            var response = await _mediator.Send(new CreateAddressCommand(CustomerId,
                                                                          request.State,
                                                                          request.PostalCode,
                                                                          request.HouseNo,
                                                                          request.Street,
                                                                          request.Area,
                                                                          request.Province,
                                                                          request.City,
                                                                          request.Country));

            return response.IsSuccess ? NoContent() : HandleFailure(response);

        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateAddressRequest request)
        {
            var response = await _mediator.Send(new UpdateAddressCommand(CustomerId,
                                                                         id,
                                                                         request.State,
                                                                         request.PostalCode,
                                                                         request.HouseNo,
                                                                         request.Street,
                                                                         request.Area,
                                                                         request.Province,
                                                                         request.City,
                                                                         request.Country));

            return response.IsSuccess ? NoContent() : HandleFailure(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _mediator.Send(new DeleteAddressCommand(CustomerId, id));

            return response.IsSuccess ? NoContent() : HandleFailure(response);
        }
        
    }
}
