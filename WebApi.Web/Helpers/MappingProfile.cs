using System;
using AutoMapper;
using WebApi.Core.Entities;
using WebApi.Infrastructure.Identity;
using WebApi.Web.Helpers.Exceptions;
using WebApi.Web.Models;

namespace WebApi.Web.Helpers
{
  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
      CreateMap<Customer, CustomerDTO>();
      CreateMap<CustomerDTO, Customer>();
      CreateMap<RegisterModel, ApplicationUser>();

      CreateMap<Exception, ErrorModel>();
    }
  }
}
