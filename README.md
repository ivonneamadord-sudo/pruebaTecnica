Prueba Técnica - Conocimientos .NET 

Este proyecto es una API RESTful desarrollada en .NET 8 como parte de una prueba técnica.  
La API permite gestionar clientes y cuentas bancarias, realizar operaciones básicas y ejecutar pruebas unitarias para validar la lógica de negocio.

---
Características principales
- Creación y consulta de clientes
- Apertura y consulta de cuentas por cliente
- Operaciones de depósito, retiros y aplicación de intereses
- Validación de entradas y manejo de errores controlados
- Pruebas unitarias con **xUnit** para cubrir casos de éxito y error

Las pruebas unitarias están ubicadas en el proyecto BancoAPI.Tests, que contiene todos los tests desarrollados con xUnit para validar la funcionalidad de la API. 
Aquí se cubren casos de éxito, manejo de errores y validaciones de entrada para asegurar la robustez del sistema.

Cada carpeta dentro de BancoAPI.Tests está organizada según la funcionalidad que prueba, facilitando la navegación y mantenimiento de las pruebas.
