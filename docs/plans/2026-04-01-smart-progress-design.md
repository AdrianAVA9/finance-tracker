# Diseño: Gestión Inteligente de Estado de Progreso (Smart Progress)

Este documento detalla la lógica de visualización dinámica para el estado de progreso en las tarjetas de presupuesto de CeroBase.

## Objetivo
Transformar el indicador de progreso en una herramienta de decisión activa mediante el uso de colores de semáforo y mensajes de disponibilidad en lugar de solo datos de consumo bruto.

## Lógica de Color (Semáforo 🚦)
El color de la barra de progreso y del texto de estado cambiará dinámicamente según el `spentRatio` (consumo / límite):

- **Zona Segura (Verde Esmeralda - #10b981)**: Consumo entre 0% y 80%. Indica que el gasto está bajo control.
- **Zona de Precaución (Ámbar/Naranja - #f59e0b)**: Consumo entre 80% y 99%. Alerta al usuario de que se acerca al límite.
- **Zona Crítica (Rojo - #ef4444)**: Consumo del 100% o superior. Indica que el presupuesto se ha agotado o excedido.

## Mensajería de Disponibilidad (Actionable Text)
Se prioriza la información de "capacidad de gasto" sobre el dato histórico de consumo:

- **Bajo Presupuesto**: `"₡X Disponibles de ₡Y"`
- **Excedido**: `"₡X Excedidos de ₡Y"` (en color rojo)

## Ventajas
- **Reducción de Carga Cognitiva**: El usuario entiende su situación financiera por el color antes de leer los números.
- **Foco en el Futuro**: Se resalta cuánto "queda" por gastar, que es el dato más relevante para la toma de decisiones diaria.
- **Unificación de Datos**: Se muestra el límite total (`₡Y`) sin necesidad de duplicarlo en otras partes de la tarjeta.

---

## Plan de Implementación (Resumen)
1. Modificar los `computed` en `BudgetItemCard.vue` para manejar los 3 estados de color.
2. Crear un nuevo `computed` para el mensaje de estado dinámico.
3. Actualizar el template de `BudgetItemCard.vue` para inyectar estas propiedades.

**Aprobado por el usuario el 2026-04-01.**
