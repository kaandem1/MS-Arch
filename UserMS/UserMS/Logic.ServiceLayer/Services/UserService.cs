using UserMS.Core.DomainLayer.Configuration;
using UserMS.Core.DomainLayer.Models;
using UserMS.Data.RepositoryLayer.IRepository;
using UserMS.Logic.ServiceLayer.Helpers;
using UserMS.Logic.ServiceLayer.IServices;
using Microsoft.Extensions.Options;
using UserMS.Core.DomainLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;


namespace UserMS.Logic.ServiceLayer.Services
{
    public class UserService : IUserService
    {
        IUserRepository _userRepository;
        private PasswordEncryptionOptions _options;
        private readonly HttpClient _httpClient;


        public UserService(IUserRepository userRepository, IOptions<PasswordEncryptionOptions> options, HttpClient httpClient)
        {
            _userRepository = userRepository;
            _options = options.Value;
            _httpClient = httpClient;
        }

        public async Task<User> GetAsync(int id)
        {
            return await _userRepository.FindByIdAsync(id);
        }
        public async Task<List<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            using (var transaction = await _userRepository.BeginTransactionAsync())
            {
                try
                {
                    user.Password = PasswordHelper.Hash(_options.StaticSalt, user.Password);
                    await _userRepository.InsertAsync(user);

                    int newUserId = user.Id;

                    var content = new StringContent(JsonSerializer.Serialize(new { userId = newUserId }), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync("http://host.docker.internal/api/PersonReference", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Failed to create person reference");
                    }

                    await transaction.CommitAsync();
                    return user;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Transaction failed: {ex.Message}");
                }
            }
        }


        public async Task DeleteAsync(int id)
        {
            using (var transaction = await _userRepository.BeginTransactionAsync())
            {
                try
                {
                    //var responseClearDevices = await _httpClient.PatchAsync($"https://device-ms:7155/api/Device/clear/{id}", null);
                    var responseClearDevices = await _httpClient.PatchAsync($"http://host.docker.internal/api/Device/clear/{id}", null);

                    if (!responseClearDevices.IsSuccessStatusCode)
                    {
                        throw new Exception("Failed to clear devices for the user.");
                    }

                    await _userRepository.DeleteByIdAsync(id);
                    //var responseDelete = await _httpClient.DeleteAsync($"https://device-ms:7155/api/PersonReference/{id}");
                    var responseDelete = await _httpClient.DeleteAsync($"http://host.docker.internal/api/PersonReference/{id}");

                    if (!responseDelete.IsSuccessStatusCode)
                    {
                        throw new Exception("Failed to delete person reference");
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Transaction failed: {ex.Message}");
                }
            }
        }

        public async Task UpdateAsync(User user)
        {

            await _userRepository.UpdateAsync(user);

        }

        public async Task<User> FindByCredentialsAsync(string email, string password)
        {
            string hashedPassword = PasswordHelper.Hash(_options.StaticSalt, password);
            return await _userRepository.FindByCredentials(email, hashedPassword);
        }
        public async Task<User> FindByEmailAsync(string email)
        {
            return await _userRepository.FindByEmailAsync(email);
        }


        public async Task<User> ChangePasswordAsync(User user, string newPassword)
        {
            string hashedPassword = PasswordHelper.Hash(_options.StaticSalt, newPassword);
            user.Password = hashedPassword;
            await _userRepository.UpdateAsync(user);
            return user;
        }
        public async Task ResetPasswordAsync(string email, string newPassword)
        {
            string hashedPassword = PasswordHelper.Hash(_options.StaticSalt, newPassword);
            User user = await _userRepository.FindByEmailAsync(email);
            if (user != null)
            {
                user.Password = hashedPassword;
                await _userRepository.UpdateAsync(user);
            }
        }


        public Task<List<User>> GetAllUserAsync()
        {
            throw new NotImplementedException();
        }


        public async Task<IEnumerable<User>> SearchByNameAsync(string name)
        {
            return await _userRepository.SearchByNameAsync(name);
        }

    }
}
