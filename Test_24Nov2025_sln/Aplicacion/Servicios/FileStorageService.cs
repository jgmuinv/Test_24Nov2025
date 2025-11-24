using Aplicacion.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Aplicacion.Servicios
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _rootPath;

        public FileStorageService(string rootPath)
        {
            _rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            Directory.CreateDirectory(_rootPath);
        }

        public async Task<string> SaveProductImageAsync(Stream fileStream, string fileName, CancellationToken ct = default)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("Stream vacío", nameof(fileStream));

            var ext = Path.GetExtension(fileName);
            var name = $"prod_{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(_rootPath, name);

            await using (var stream = new FileStream(fullPath, FileMode.CreateNew))
            {
                await fileStream.CopyToAsync(stream, ct);
            }

            return name; // retornar solo el nombre del archivo
        }
    }
}
