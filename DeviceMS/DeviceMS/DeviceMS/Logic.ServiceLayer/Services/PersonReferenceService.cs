using Core.DomainLayer.Configuration;
using DeviceMS.Core.DomainLayer.Models;
using DeviceMS.Data.RepositoryLayer.IRepository;
using DeviceMS.Logic.ServiceLayer.Helpers;
using DeviceMS.Logic.ServiceLayer.IServices;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Collections.Generic;
using DeviceMS.Data.RepositoryLayer.Repository;

namespace DeviceMS.Logic.ServiceLayer.Services
{
    public class PersonReferenceService : IPersonReferenceService
    {
        IPersonReferenceRepository _personReferenceRepository;

        public PersonReferenceService(IPersonReferenceRepository personReferenceRepository)
        {
            _personReferenceRepository = personReferenceRepository;
        }

        public async Task<PersonReference> GetAsync(int id)
        {
            return await _personReferenceRepository.GetByIdAsync(id);
        }

        public async Task<List<PersonReference>> GetAllAsync()
        {
            return await _personReferenceRepository.GetAllAsync();
        }

        public async Task<PersonReference> CreateAsync(PersonReference person)
        {
            return await _personReferenceRepository.InsertAsync(person);
        }
        
        public async Task DeleteAsync(PersonReference person)
        {
            await _personReferenceRepository.DeleteByIdAsync(person.UserId);
        }
    }
}
