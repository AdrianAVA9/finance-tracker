# Proceso de Re-diseño: Indicador de Recurrencia (Badge de Estado)

Este documento detalla el diseño para el indicador de presupuestos recurrentes en la aplicación CeroBase, optimizando la visibilidad sin comprometer la limpieza de la interfaz.

## Objetivo
Transformar el indicador de texto/icono de "Recurrente" en un badge de estado visualmente integrado al icono de la categoría.

## Diseño Propuesto (Opción 1: Status Badge)
- **Componente**: `BudgetItemCard.vue`.
- **Estructura**:
    - El contenedor del icono de la categoría (`w-12 h-12`) se vuelve `relative`.
    - Se añade un badge circular absoluto en la esquina inferior derecha (`bottom-0 right-0`).
    - **Estilo**: Fondo del color primario (verde), con un icono de sincronización blanco minúsculo.
    - **Comportamiento**: Solo visible cuando `budget.isRecurrent` es verdadero.

## Ventajas
- **Eficiencia Espacial**: No consume espacio horizontal en el nombre de la categoría.
*   **Intuición**: Sigue patrones modernos de diseño de "badges de estado" (como los indicadores de "online" en apps de chat).
- **Aesthetic**: Mantiene el look premium y minimalista de CeroBase.

---

## Plan de Implementación (Resumen)
1.  Modificar `BudgetItemCard.vue`.
2.  Actualizar el contenedor del icono con `relative`.
3.  Inyectar el badge absoluto.
4.  Remover el icono previo junto al nombre de la categoría.

**Aprobado por el usuario el 2026-04-01.**
