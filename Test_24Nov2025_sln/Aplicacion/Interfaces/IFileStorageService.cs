using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Aplicacion.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveProductImageAsync(Stream fileStream, string fileName, CancellationToken ct = default);
    }
}
