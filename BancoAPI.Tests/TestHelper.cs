using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BancoAPI.Tests
{
    public static class TestHelper
    {
        public static PDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            return new PDbContext(options);
        }
    }
}

