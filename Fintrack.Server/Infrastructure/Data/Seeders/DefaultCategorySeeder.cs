using System;
using Fintrack.Server.Infrastructure.Data;
using Fintrack.Server.Domain.Abstractions;
using Fintrack.Server.Domain.Budgets;
using Fintrack.Server.Domain.Enums;
using Fintrack.Server.Domain.Exceptions;
using Fintrack.Server.Domain.ExpenseCategories;
using Fintrack.Server.Domain.Expenses;
using Fintrack.Server.Domain.IncomeCategories;
using Fintrack.Server.Domain.SavingsGoals;
using Fintrack.Server.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Fintrack.Server.Infrastructure.Data.Seeders
{
    public static class DefaultCategorySeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await SeedExpenseCategoriesAsync(context);
            await SeedIncomeCategoriesAsync(context);
        }

        private static async Task SeedExpenseCategoriesAsync(ApplicationDbContext context)
        {
            if (await context.ExpenseCategoryGroups.AnyAsync(g => g.UserId == null))
            {
                return;
            }

            var basicGroupId = await TryAddGroup(context, "BÁSICOS MENSUALES", "El costo fijo de vida");
            var transitGroupId = await TryAddGroup(context, "TRANSPORTE Y AUTOMÓVIL", "Todo lo que te mueve");
            var foodGroupId = await TryAddGroup(context, "ALIMENTACIÓN", "Comida fuera de casa");
            var healthGroupId = await TryAddGroup(context, "SALUD Y CUIDADO PERSONAL", "Cuidado físico y estético");
            var homeFamilyGroupId = await TryAddGroup(context, "CASA Y FAMILIA", "Mantenimiento del hogar y educación personal");
            var kidsGroupId = await TryAddGroup(context, "HIJOS", "Gastos y educación de hijos");
            var petsGroupId = await TryAddGroup(context, "MASCOTAS", "Alimento, salud y accesorios de mascotas");
            var insuranceGroupId = await TryAddGroup(context, "SEGUROS", "Pólizas y protección");
            var funGroupId = await TryAddGroup(context, "ENTRETENIMIENTO Y OCIO", "Viajes, salidas, suscripciones y regalos");
            var debtGroupId = await TryAddGroup(context, "DEUDAS", "Préstamos y tarjetas de crédito");
            var savingsGroupId = await TryAddGroup(context, "AHORROS E INVERSIONES", "Fondos de emergencia, vivienda, metas a largo plazo");

            await context.SaveChangesAsync();

            // 1. Básicos Mensuales
            await TryAddCategory(context, "Arriendo / Hipoteca", "Pago mensual del alquiler o la cuota del préstamo de la vivienda.", basicGroupId, "house", "#4F46E5");
            await TryAddCategory(context, "Teléfono / Celular", "Recibos de planes móviles y recargas.", basicGroupId, "smartphone", "#8B5CF6");
            await TryAddCategory(context, "Electricidad", "Recibo de luz mensual.", basicGroupId, "bolt", "#F59E0B");
            await TryAddCategory(context, "Agua", "Recibo de acueductos y alcantarillados.", basicGroupId, "water_drop", "#3B82F6");
            await TryAddCategory(context, "Gas", "Compra de cilindros de gas para cocina o calentador.", basicGroupId, "local_gas_station", "#EF4444");
            await TryAddCategory(context, "Cable / Internet", "Servicio de internet residencial y televisión.", basicGroupId, "wifi", "#10B981");
            await TryAddCategory(context, "Supermercado", "Compras de despensa, abarrotes, artículos de limpieza y necesidades del hogar.", basicGroupId, "shopping_cart", "#F43F5E");

            // 2. Transporte y Automóvil
            await TryAddCategory(context, "Gasolina", "Combustible para uso del vehículo.", transitGroupId, "local_gas_station", "#F59E0B");
            await TryAddCategory(context, "Transporte Público", "Pasajes de bus, tren o transportes masivos.", transitGroupId, "directions_bus", "#64748B");
            await TryAddCategory(context, "Uber / Taxi", "Servicios de transporte privado mediante plataformas.", transitGroupId, "local_taxi", "#FCD34D");
            await TryAddCategory(context, "Estacionamiento / Peajes", "Pago de parqueos, cuidacarros, peajes y QuickPass/Compass.", transitGroupId, "toll", "#94A3B8");
            await TryAddCategory(context, "Mantenimiento Auto", "Cambios de aceite, llantas, lavado, repuestos y revisiones.", transitGroupId, "build", "#64748B");
            await TryAddCategory(context, "Marchamo e Impuestos", "Pago anual del derecho de circulación o revisiones técnicas.", transitGroupId, "description", "#475569");

            // 3. Alimentación
            await TryAddCategory(context, "Restaurantes", "Comidas completas fuera de casa, cenas y pedidos por delivery.", foodGroupId, "restaurant", "#F43F5E");
            await TryAddCategory(context, "Cafeterías y Snacks", "Compras rápidas, cafés, panaderías o tiendas de conveniencia.", foodGroupId, "local_cafe", "#D97706");

            // 4. Salud y Cuidado Personal
            await TryAddCategory(context, "Medicinas y Farmacia", "Compra de medicamentos, vitaminas y botiquín.", healthGroupId, "medication", "#10B981");
            await TryAddCategory(context, "Consultas Médicas", "Visitas al médico general, especialistas, dentista o exámenes.", healthGroupId, "health_and_safety", "#059669");
            await TryAddCategory(context, "Nutrición y Fitness", "Mensualidad del gimnasio, deportes y consultas nutricionales.", healthGroupId, "fitness_center", "#3B82F6");
            await TryAddCategory(context, "Cuidado Personal", "Cortes de cabello, barbería, cosméticos y productos de higiene personal.", healthGroupId, "spa", "#EC4899");
            await TryAddCategory(context, "Ropa y Calzado", "Compra de vestimenta, zapatos y accesorios personales.", healthGroupId, "checkroom", "#8B5CF6");
            await TryAddCategory(context, "Lavandería y Sastrería", "Servicios de tintorería y arreglo de ropa.", healthGroupId, "local_laundry_service", "#64748B");

            // 5. Casa y Familia
            await TryAddCategory(context, "Mantenimiento del Hogar", "Reparaciones, pintura, ferretería y arreglos de la vivienda.", homeFamilyGroupId, "home_repair_service", "#F59E0B");
            await TryAddCategory(context, "Mobiliario y Electrodomésticos", "Compra de muebles, decoración y aparatos para la casa.", homeFamilyGroupId, "chair", "#FCD34D");
            await TryAddCategory(context, "Impuestos Territoriales", "Pago de bienes inmuebles o tributos municipales.", homeFamilyGroupId, "account_balance", "#475569");
            await TryAddCategory(context, "Educación Personal", "Pagos de universidad, certificaciones o cursos propios.", homeFamilyGroupId, "school", "#3B82F6");

            // 6. Hijos
            await TryAddCategory(context, "Ropa y Calzado (Hijos)", "Vestimenta exclusiva para los hijos.", kidsGroupId, "child_care", "#EC4899");
            await TryAddCategory(context, "Educación y Útiles", "Matrículas, mensualidades escolares, libros y papelería.", kidsGroupId, "backpack", "#8B5CF6");
            await TryAddCategory(context, "Cuido / Niñera", "Pago por servicios de cuidado infantil o guardería.", kidsGroupId, "baby_changing_station", "#F43F5E");

            // 7. Mascotas
            await TryAddCategory(context, "Comida Mascota", "Alimento seco, húmedo y premios.", petsGroupId, "pets", "#D97706");
            await TryAddCategory(context, "Salud Veterinaria", "Consultas, vacunas, desparasitantes y medicinas.", petsGroupId, "medical_services", "#10B981");
            await TryAddCategory(context, "Accesorios Mascota", "Camas, correas, juguetes y arena.", petsGroupId, "category", "#F59E0B");

            // 8. Seguros
            await TryAddCategory(context, "Seguro de Vehículo", "Póliza voluntaria del carro.", insuranceGroupId, "car_crash", "#ef4444");
            await TryAddCategory(context, "Seguro de Vida", "Póliza de vida personal.", insuranceGroupId, "favorite", "#F43F5E");
            await TryAddCategory(context, "Seguro de Vivienda", "Póliza contra incendios o robos.", insuranceGroupId, "security", "#3B82F6");
            await TryAddCategory(context, "Seguro Médico", "Gastos médicos mayores o planes preventivos de salud.", insuranceGroupId, "health_and_safety", "#10B981");

            // 9. Entretenimiento y Ocio
            await TryAddCategory(context, "Vacaciones y Viajes", "Tiquetes, hospedaje, tours y viáticos.", funGroupId, "flight_takeoff", "#3B82F6");
            await TryAddCategory(context, "Salidas y Eventos", "Entradas a conciertos, cine, bares y discotecas.", funGroupId, "celebration", "#8B5CF6");
            await TryAddCategory(context, "Suscripciones Digitales", "Pagos recurrentes de plataformas como Netflix, Spotify, iCloud o software.", funGroupId, "subscriptions", "#EC4899");
            await TryAddCategory(context, "Regalos", "Obsequios para familiares, amigos y celebraciones.", funGroupId, "card_giftcard", "#F43F5E");
            await TryAddCategory(context, "Caridad y Donaciones", "Apoyo económico a instituciones o personas.", funGroupId, "volunteer_activism", "#10B981");

            // 10. Deudas
            await TryAddCategory(context, "Préstamo Personal", "Cuotas de créditos de libre inversión.", debtGroupId, "account_balance_wallet", "#EF4444");
            await TryAddCategory(context, "Crédito Comercial", "Cuotas por compras financiadas en tiendas de electrodomésticos.", debtGroupId, "storefront", "#ef4444");
            await TryAddCategory(context, "Tarjeta de Crédito", "Abonos mensuales a la tarjeta.", debtGroupId, "credit_card", "#ef4444");
            await TryAddCategory(context, "Préstamo Vehículo", "Cuota prendaria del carro.", debtGroupId, "directions_car", "#EF4444");
            await TryAddCategory(context, "Otras Deudas", "Pagos a prestamistas u obligaciones no clasificadas.", debtGroupId, "money_off", "#EF4444");

            // 11. Ahorros e Inversiones
            await TryAddCategory(context, "Fondo de Emergencias", "Ahorro para eventualidades, imprevistos o cobertura de necesidades básicas.", savingsGroupId, "savings", "#10B981");
            await TryAddCategory(context, "Fondo Patrimonial", "Inversión a largo plazo para crecimiento del capital.", savingsGroupId, "trending_up", "#059669");
            await TryAddCategory(context, "Ahorro Educación", "Fondos reservados para futuros estudios.", savingsGroupId, "school", "#3B82F6");
            await TryAddCategory(context, "Ahorro Vivienda", "Dinero destinado para la prima de una casa.", savingsGroupId, "home", "#4F46E5");
            await TryAddCategory(context, "Ahorro Vehículo", "Fondos para cambiar o comprar un carro.", savingsGroupId, "directions_car", "#3B82F6");
            await TryAddCategory(context, "Otros Objetivos", "Ahorros específicos a corto plazo.", savingsGroupId, "flag", "#8B5CF6");

            await context.SaveChangesAsync();
        }

        private static async Task SeedIncomeCategoriesAsync(ApplicationDbContext context)
        {
            // Clear existing system categories to ensure we have exactly the ones requested
            var existingSystemCategories = await context.IncomeCategories
                .Where(c => c.UserId == null)
                .ToListAsync();
            
            if (existingSystemCategories.Any())
            {
                context.IncomeCategories.RemoveRange(existingSystemCategories);
                await context.SaveChangesAsync();
            }

            // --- Income Categories ---
            await TryAddIncomeCategory(context, "Salario / Nómina", "payments", "#10B981");
            await TryAddIncomeCategory(context, "Freelance / Consultoría", "work", "#06B6D4");
            await TryAddIncomeCategory(context, "Ventas / Negocios", "storefront", "#F59E0B");
            await TryAddIncomeCategory(context, "Bonos y Aguinaldo", "redeem", "#EC4899");
            await TryAddIncomeCategory(context, "Inversiones", "trending_up", "#8B5CF6");
            await TryAddIncomeCategory(context, "Rentas", "apartment", "#3B82F6");
            await TryAddIncomeCategory(context, "Reembolsos", "assignment_return", "#64748B");
            await TryAddIncomeCategory(context, "Otros", "more_horiz", "#94A3B8");

            await context.SaveChangesAsync();
        }

        private static async Task<Guid> TryAddGroup(ApplicationDbContext context, string name, string description)
        {
            var createResult = ExpenseCategoryGroup.CreateForSystem(name, description);
            if (createResult.IsFailure)
            {
                throw new InvalidOperationException(createResult.Error.Description);
            }

            var group = createResult.Value;

            context.ExpenseCategoryGroups.Add(group);
            await context.SaveChangesAsync(); // save immediately to get ID for linking to categories
            return group.Id;
        }

        private static Task TryAddCategory(ApplicationDbContext context, string name, string description, Guid groupId, string icon, string color)
        {
            var createResult = ExpenseCategory.Create(
                name,
                description,
                icon,
                color,
                groupId,
                userId: null,
                isEditable: false);

            if (createResult.IsFailure)
            {
                throw new InvalidOperationException(createResult.Error.Description);
            }

            context.ExpenseCategories.Add(createResult.Value);
            return Task.CompletedTask;
        }

        private static Task TryAddIncomeCategory(ApplicationDbContext context, string name, string icon, string color)
        {
            var createResult = IncomeCategory.Create(
                name,
                icon,
                color,
                userId: null,
                isEditable: false);

            if (createResult.IsFailure)
            {
                throw new InvalidOperationException(createResult.Error.Description);
            }

            context.IncomeCategories.Add(createResult.Value);
            return Task.CompletedTask;
        }
    }
}
