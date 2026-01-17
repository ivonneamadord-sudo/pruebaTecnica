using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Data;
using PruebaTecnica.Models;
using PruebaTecnica.Services;


namespace BancoAPI.Tests
{
    public class ClienteTests
    {

        //************CREAR CLIENTE CON TODOS LOS DATOS REQUERIDOS************
        [Fact]
        public void CrearCliente_DeberiaGuardarClienteConTodosLosCampos()
        {
           
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            using var context = new PDbContext(options);

            
            var cliente = new Cliente
            {
                DNI = "0801199407980",
                nombre = "Ivonne Amador",
                fechaNacimiento = DateOnly.FromDateTime(new DateTime(1994, 5, 1)),
                sexo = "F",
                ingresos = 50000
            };

            context.Clientes.Add(cliente);
            context.SaveChanges();

            
            var clienteGuardado = context.Clientes.FirstOrDefault(c => c.DNI == "0801199407980");

            Assert.NotNull(clienteGuardado);                        
            Assert.Equal("Ivonne Amador", clienteGuardado.nombre);
            Assert.Equal(DateOnly.FromDateTime(new DateTime(1994, 5, 1)), clienteGuardado.fechaNacimiento);
            Assert.Equal("F", clienteGuardado.sexo);                
            Assert.Equal(50000, clienteGuardado.ingresos);          
        }



        //************CREAR CLIENTE CON VALORES INCOMPLETOS************
        [Fact]
        public void CrearCliente_SinDatosCompletos_NoGuarda_LanzaMensaje()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_Incompleto")
                .Options;

            using var context = new PDbContext(options);

            var cliente = new Cliente
            {
                DNI = "0801199407980",
                sexo = "F"
            };

            context.Clientes.Add(cliente);

            var ex = Record.Exception(() => context.SaveChanges());

            Assert.NotNull(ex); 
        }



        //************CREAR CLIENTE DUPLICADO************
        [Fact]
        public void CrearCliente_Duplicado_DeberiaLanzarExcepcion_LanzaMensajeDeDuplicado()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase("TestDB_Duplicado")
                .Options;

            using var context = new PDbContext(options);
            var service = new ClientService(context);

            var cliente = new Cliente
            {
                DNI = "0801199407980",
                nombre = "Ivonne Amador",
                fechaNacimiento = DateOnly.FromDateTime(new DateTime(1994, 5, 1)),
                sexo = "F",
                ingresos = 50000
            };

            service.CrearCliente(cliente);

            cliente = new Cliente
            {
                DNI = "0801199407980", 
                nombre = "Ivonne Amador",
                fechaNacimiento = DateOnly.FromDateTime(new DateTime(1994, 5, 1)),
                sexo = "F",
                ingresos = 50000
            };

            var ex = Assert.Throws<InvalidOperationException>(() => service.CrearCliente(cliente));

            Assert.Equal("Ya existe un cliente con el DNI 0801199407980", ex.Message);
        }



        //************CONSULTAR SI EXISTE CLIENTE POR DNI************
        [Fact]
        public void ConsultarClientePorId_Existe_DeberiaRetornarCliente()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase("TestDB_ConsultaExiste")
                .Options;

            using var context = new PDbContext(options);
            var service = new ClientService(context);

            var cliente = new Cliente
            {
                DNI = "0801199407980",
                nombre = "Ivonne Amador",
                fechaNacimiento = DateOnly.FromDateTime(new DateTime(1994, 5, 1)),
                sexo = "F",
                ingresos = 50000
            };
            service.CrearCliente(cliente);

            var resultado = service.ObtenerClientePorDNI(cliente.DNI);

            Assert.NotNull(resultado);
            Assert.Equal("Ivonne Amador", resultado.nombre);
        }



        //************CONSULTAR SI EXISTE CLIENTE POR DNI - CLIENTE NO EXISTENTE EN BASE************
        [Fact]
        public void ConsultarClientePorId_NoExiste_DeberiaRetornarNull_MensajeNoEncontro()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase("TestDB_ConsultaNoExiste")
                .Options;

            using var context = new PDbContext(options);
            var service = new ClientService(context);

            var resultado = service.ObtenerClientePorDNI("999");

            Assert.Null(resultado);
        }



    }

    public class CuentaTests
    {
        //************CREAR CUENTA POR DNI EXISTENTE************
        [Fact]
        public void CrearCuentaPorDni_ClienteExiste_DeberiaCrearCuenta()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase("TestDB_CrearCuentaPorDni")
                .Options;

            using var context = new PDbContext(options);
            var clienteService = new ClientService(context);
            var cuentaService = new AccountService(context);

            var cliente = new Cliente
            {
                DNI = "0801199407980",
                nombre = "Ivonne Amador",
                fechaNacimiento = DateOnly.FromDateTime(new DateTime(1994, 5, 1)),
                sexo = "F",
                ingresos = 50000
            };
            clienteService.CrearCliente(cliente);

            var cuenta = cuentaService.CrearCuenta("0801199407980", 1000);

            Assert.NotNull(cuenta);
            Assert.Equal(cliente.Id, cuenta.clienteId);
            Assert.Equal(1000, cuenta.saldo);
        }



        //************CREAR CUENTA POR DNI - NO EXISTENTE************
        [Fact]
        public void CrearCuentaPorDni_ClienteNoExiste_DeberiaLanzarExcepcion()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase("TestDB_CrearCuentaPorDniInvalido")
                .Options;

            using var context = new PDbContext(options);
            var cuentaService = new AccountService(context);

            var ex = Assert.Throws<InvalidOperationException>(() =>
                cuentaService.CrearCuenta("9999999999999", 1000));

            Assert.Throws<InvalidOperationException>(() =>
            cuentaService.CrearCuenta("9999999999999", 500));

        }



        //************CONSULTAR SALDO POR NUMERO DE CUENTA EXISTENTE************
        [Fact]
        public void ConsultarSaldoPorNumeroCuenta_CuentaExiste_DeberiaRetornarSaldo()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase("TestDB_SaldoPorCuentaExiste")
                .Options;

            using var context = new PDbContext(options);
            var clienteService = new ClientService(context);
            var cuentaService = new AccountService(context);

            var cliente = new Cliente
            {
                DNI = "0801199407980",
                nombre = "Ivonne Amador",
                fechaNacimiento = DateOnly.FromDateTime(new DateTime(1994, 5, 1)),
                sexo = "F",
                ingresos = 50000
            };
            clienteService.CrearCliente(cliente);

            var cuenta = new Cuenta
            {
                clienteId = cliente.Id,
                numeroCuenta = "1234567890",
                saldo = 2500
            };
            context.Cuentas.Add(cuenta);
            context.SaveChanges();

            var saldo = cuentaService.ConsultarSaldo("1234567890");

            Assert.Equal(2500, saldo);
        }



        //************CONSULTAR SALDO POR NUMERO DE CUENTA NO EXISTENTE************
        [Fact]
        public void ConsultarSaldoPorNumeroCuenta_CuentaNoExiste_DeberiaRetornarCero()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PDbContext(options);
            var cuentaService = new AccountService(context);

            var saldo = cuentaService.ConsultarSaldo("CUENTA_INEXISTENTE_001");

            Assert.Equal(0, saldo);
        }




        //************CONSULTAR CUENTAS POR IDCLIENTE CON CUENTAS************
        [Fact]
        public void ConsultarCuentasPorClienteId_ClienteConCuentas_DeberiaRetornarLista()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PDbContext(options);
            var clienteService = new ClientService(context);
            var cuentaService = new AccountService(context);

            var cliente = new Cliente
            {
                DNI = "0801199407980",
                nombre = "Ivonne Amador",
                fechaNacimiento = DateOnly.FromDateTime(new DateTime(1994, 5, 1)),
                sexo = "F",
                ingresos = 50000
            };
            clienteService.CrearCliente(cliente);

            var cuenta1 = new Cuenta { clienteId = cliente.Id, numeroCuenta = "111", saldo = 1000 };
            var cuenta2 = new Cuenta { clienteId = cliente.Id, numeroCuenta = "222", saldo = 2000 };
            context.Cuentas.AddRange(cuenta1, cuenta2);
            context.SaveChanges();

            var cuentas = cuentaService.ObtenerCuentasPorClienteId(cliente.Id);

            Assert.NotNull(cuentas);
            Assert.Equal(2, cuentas.Count);
            Assert.Contains(cuentas, c => c.numeroCuenta == "111");
            Assert.Contains(cuentas, c => c.numeroCuenta == "222");
        }



        //************CONSULTAR CUENTAS POR IDCLIENTE - SIN CUENTAS************
        [Fact]
        public void ConsultarCuentasPorClienteId_ClienteSinCuentas_DeberiaLanzarExcepcion()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PDbContext(options);
            var cuentaService = new AccountService(context);

            var ex = Assert.Throws<InvalidOperationException>(() =>
                cuentaService.ObtenerCuentasPorClienteId(999));

            Assert.Equal("No existe un cliente con Id 999", ex.Message);
        }



        //************CONSULTAR CUENTAS POR IDCLIENTE - NO EXISTE IDCLIENTE************
        [Fact]
        public void ConsultarCuentasPorClienteId_IdNoExiste_DeberiaLanzarExcepcion()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            using var context = new PDbContext(options);
            var cuentaService = new AccountService(context);

            // Act + Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
                cuentaService.ObtenerCuentasPorClienteId(9999)); 

            Assert.Equal("No existe un cliente con Id 9999", ex.Message);
        }



        //************CONSULTAR CUENTAS POR DNI CON CUENTAS************
        [Fact]
        public void ConsultarCuentasPorDni_ClienteConCuentas_DeberiaRetornarLista()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PDbContext(options);
            var clienteService = new ClientService(context);
            var cuentaService = new AccountService(context);

            // Arrange: crear cliente
            var cliente = new Cliente
            {
                DNI = "0801199407980",
                nombre = "Ivonne Amador",
                fechaNacimiento = DateOnly.FromDateTime(new DateTime(1994, 5, 1)),
                sexo = "F",
                ingresos = 50000
            };
            clienteService.CrearCliente(cliente);

            // Crear cuentas asociadas
            var cuenta1 = new Cuenta { clienteId = cliente.Id, numeroCuenta = "111", saldo = 1000 };
            var cuenta2 = new Cuenta { clienteId = cliente.Id, numeroCuenta = "222", saldo = 2000 };
            context.Cuentas.AddRange(cuenta1, cuenta2);
            context.SaveChanges();

            // Act
            var cuentas = cuentaService.ObtenerCuentasPorDNI("0801199407980");

            // Assert
            Assert.NotNull(cuentas);
            Assert.Equal(2, cuentas.Count);
        }


        //************CONSULTAR CUENTAS POR DNI SIN CUENTAS************
        [Fact]
        public void ConsultarCuentasPorDni_ClienteSinCuentas_DeberiaRetornarListaVacia()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PDbContext(options);
            var clienteService = new ClientService(context);
            var cuentaService = new AccountService(context);

            var cliente = new Cliente
            {
                DNI = "0801199407999",
                nombre = "Cliente Sin Cuentas",
                fechaNacimiento = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
                sexo = "M",
                ingresos = 30000
            };
            clienteService.CrearCliente(cliente);

            var cuentas = cuentaService.ObtenerCuentasPorDNI("0801199407999");

            Assert.NotNull(cuentas);
            Assert.Empty(cuentas); 
        }




        //************CONSULTAR CUENTAS POR DNI INEXISTENTE************
        [Fact]
        public void ConsultarCuentasPorDni_DniNoExiste_DeberiaLanzarExcepcion()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PDbContext(options);
            var cuentaService = new AccountService(context);

            // Act + Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
                cuentaService.ObtenerCuentasPorDNI("9999999999999"));

            Assert.Equal("No existe un cliente con el DNI 9999999999999", ex.Message);
        }




        //************APLICAR INTERERES A CUENTAS************
        [Fact]
        public void AplicarInteres_CuentaExiste_DeberiaActualizarSaldo()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PDbContext(options);
            var clienteService = new ClientService(context);
            var cuentaService = new AccountService(context);

            var cliente = new Cliente
            {
                DNI = "0801199407980",
                nombre = "Ivonne Amador",
                fechaNacimiento = DateOnly.FromDateTime(new DateTime(1994, 5, 1)),
                sexo = "F",
                ingresos = 50000
            };
            clienteService.CrearCliente(cliente);

            var cuenta = new Cuenta
            {
                clienteId = cliente.Id,
                numeroCuenta = "1234567890",
                saldo = 1000
            };
            context.Cuentas.Add(cuenta);
            context.SaveChanges();

            cuentaService.AplicarIntereses("1234567890", 5);

            var cuentaActualizada = context.Cuentas.First(c => c.numeroCuenta == "1234567890");
            Assert.Equal(1050, cuentaActualizada.saldo);
        }



        //************APLICAR INTERERES A CUENTAS - CUENTA NO EXISTENTE************
        [Fact]
        public void AplicarInteres_CuentaNoExiste_DeberiaLanzarExcepcion()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PDbContext(options);
            var cuentaService = new AccountService(context);

            var ex = Assert.Throws<InvalidOperationException>(() =>
                cuentaService.AplicarIntereses("CUENTA_INEXISTENTE", 5));

            Assert.Equal("No existe una cuenta con el número CUENTA_INEXISTENTE", ex.Message);
        }


    }

    public class TransaccionesTests
    {
        //************APLICAR DEPOSITO************
        [Fact]
        public void Depositar_CuentaExiste_DeberiaIncrementarSaldo()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PDbContext(options);
            var clienteService = new ClientService(context);
            var transactionService = new TransactionService(context);

            var cliente = new Cliente
            {
                DNI = "0801199407980",
                nombre = "Ivonne Amador",
                fechaNacimiento = DateOnly.FromDateTime(new DateTime(1994, 5, 1)),
                sexo = "F",
                ingresos = 50000
            };
            clienteService.CrearCliente(cliente);

            var cuenta = new Cuenta
            {
                clienteId = cliente.Id,
                numeroCuenta = "1234567890",
                saldo = 1000
            };
            context.Cuentas.Add(cuenta);
            context.SaveChanges();

            transactionService.Depositar("1234567890", 500);

            var cuentaActualizada = context.Cuentas.First(c => c.numeroCuenta == "1234567890");
            Assert.Equal(1500, cuentaActualizada.saldo);
        }



        //************APLICAR DEPOSITO - NO EXISTE CUENTA************
        [Fact]
        public void Depositar_CuentaNoExiste_DeberiaLanzarNullReferenceException()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PDbContext(options);
            var transactionService = new TransactionService(context);

            var ex = Assert.Throws<NullReferenceException>(() =>
                transactionService.Depositar("CUENTA_INEXISTENTE", 500));

            Assert.Contains("Object reference", ex.Message); 
        }



        //************APLICAR RETIRO************
        [Fact]
        public void Retirar_CuentaExiste_DeberiaDisminuirSaldo()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PDbContext(options);
            var clienteService = new ClientService(context);
            var transactionService = new TransactionService(context);

            var cliente = new Cliente
            {
                DNI = "0801199407980",
                nombre = "Ivonne Amador",
                fechaNacimiento = DateOnly.FromDateTime(new DateTime(1994, 5, 1)),
                sexo = "F",
                ingresos = 50000
            };
            clienteService.CrearCliente(cliente);

            var cuenta = new Cuenta
            {
                clienteId = cliente.Id,
                numeroCuenta = "1234567890",
                saldo = 1000
            };
            context.Cuentas.Add(cuenta);
            context.SaveChanges();

            transactionService.Retirar("1234567890", 400);

            var cuentaActualizada = context.Cuentas.First(c => c.numeroCuenta == "1234567890");
            Assert.Equal(600, cuentaActualizada.saldo);
        }



        //************APLICAR RETIRO - NO EXISTE CUENTA************
        [Fact]
        public void Retirar_CuentaNoExiste_DeberiaLanzarNullReferenceException()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            using var context = new PDbContext(options);
            var transactionService = new TransactionService(context);

            var ex = Assert.Throws<NullReferenceException>(() =>
                transactionService.Retirar("CUENTA_INEXISTENTE", 200));

            Assert.NotNull(ex); 
        }



        //************APLICAR RETIRO - FONDOS INSUFICIENTES************
        [Fact]
        public void Retirar_FondosInsuficientes_DeberiaLanzarExcepcion()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PDbContext(options);
            var clienteService = new ClientService(context);
            var transactionService = new TransactionService(context);

            var cliente = new Cliente
            {
                DNI = "0801199407999",
                nombre = "Cliente Test",
                fechaNacimiento = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
                sexo = "M",
                ingresos = 30000
            };
            clienteService.CrearCliente(cliente);

            var cuenta = new Cuenta
            {
                clienteId = cliente.Id,
                numeroCuenta = "9876543210",
                saldo = 100 
            };
            context.Cuentas.Add(cuenta);
            context.SaveChanges();

            
            var ex = Assert.Throws<InvalidOperationException>(() =>
                transactionService.Retirar("9876543210", 200)); 
            Assert.Equal("Fondos insuficientes", ex.Message);
        }



        //************HISTORIAL DE TRANSACCIONES (DEPOSITOS,RETIROS,INTERES)************
        [Fact]
        public void Transacciones_Completas_DeberianActualizarSaldoCorrectamente()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PDbContext(options);
            var clienteService = new ClientService(context);
            var transactionService = new TransactionService(context);

            // Crear cliente y cuenta con saldo inicial
            var cliente = new Cliente
            {
                DNI = "0801199407980",
                nombre = "Ivonne Amador",
                fechaNacimiento = DateOnly.FromDateTime(new DateTime(1994, 5, 1)),
                sexo = "F",
                ingresos = 50000
            };
            clienteService.CrearCliente(cliente);

            var cuenta = new Cuenta
            {
                clienteId = cliente.Id,
                numeroCuenta = "1234567890",
                saldo = 500 // saldo inicial corregido
            };
            context.Cuentas.Add(cuenta);
            context.SaveChanges();

            // Depositar 1000 → saldo esperado = 1500
            transactionService.Depositar("1234567890", 1000);
            var saldo1 = context.Cuentas.First(c => c.numeroCuenta == "1234567890").saldo;
            Assert.Equal(1500, saldo1);

            // Retirar 200 → saldo esperado = 1300
            transactionService.Retirar("1234567890", 200);
            var saldo2 = context.Cuentas.First(c => c.numeroCuenta == "1234567890").saldo;
            Assert.Equal(1300, saldo2);

            // Aplicar 10% de interés → saldo esperado = 1430

            var cuentaService = new AccountService(context);
            cuentaService.AplicarIntereses("1234567890", 10);
            var saldoFinal = context.Cuentas.First(c => c.numeroCuenta == "1234567890").saldo;
            Assert.Equal(1430, saldoFinal);
        }



        //************HISTORIAL DE TRANSACCIONES - NO EXISTE CUENTA************
        [Fact]
        public void Transacciones_Completas_CuentaNoExiste_DeberianLanzarAlgunaExcepcion()
        {
            var options = new DbContextOptionsBuilder<PDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new PDbContext(options);
            var transactionService = new TransactionService(context);
            var cuentaService = new AccountService(context);

            // Depositar
            Assert.ThrowsAny<Exception>(() =>
                transactionService.Depositar("CUENTA_INEXISTENTE", 1000));

            // Retirar
            Assert.ThrowsAny<Exception>(() =>
                transactionService.Retirar("CUENTA_INEXISTENTE", 200));

            // Aplicar interés
            Assert.ThrowsAny<Exception>(() =>
                cuentaService.AplicarIntereses("CUENTA_INEXISTENTE", 10));
        }



    }

}



