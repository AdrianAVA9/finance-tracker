<template>
    <div class="pricing-page container">
        <section class="pricing-header">
            <h1>Simple, Transparent Pricing</h1>
            <p>Start for free, upgrade as you grow.</p>
        </section>

        <div class="pricing-cards">
            <div class="card glass">
                <h3>Free</h3>
                <p class="price">$0<span>/mo</span></p>
                <ul>
                    <li>Basic Expense Tracking</li>
                    <li>Monthly Budget</li>
                    <li>3 Categories</li>
                </ul>
                <button class="btn-outline">Current Plan</button>
            </div>
            <div class="card glass primary">
                <h3>Pro</h3>
                <p class="price">$9<span>/mo</span></p>
                <ul>
                    <li>Unlimited Expenses</li>
                    <li>AI Invoice Scanning</li>
                    <li>Advanced Reports</li>
                </ul>
                <button class="btn-primary">Upgrade</button>
            </div>
        </div>
    </div>
</template>

<style scoped lang="scss">
.pricing-header {
    text-align: center;
    padding: 4rem 1rem;
}

.pricing-cards {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
    gap: 2rem;
    padding: 0 1rem 4rem;
}

.card {
    padding: 2rem;
    border-radius: 1rem;
    text-align: center;
    transition: transform 0.2s;

    &:hover {
        transform: translateY(-5px);
    }

    &.primary {
        border: 2px solid hsl(var(--color-primary));
    }

    .price {
        font-size: 2.5rem;
        font-weight: 800;
        margin: 1rem 0;

        span {
            font-size: 1rem;
            font-weight: 400;
            color: #666;
        }
    }

    ul {
        list-style: none;
        margin-bottom: 2rem;

        li {
            margin-bottom: 0.5rem;
        }
    }

    button {
        width: 100%;
        padding: 0.75rem;
        border-radius: 0.5rem;
        font-weight: bold;
        cursor: pointer;
    }

    .btn-primary {
        background: hsl(var(--color-primary));
        color: white;
        border: none;
    }

    .btn-outline {
        background: transparent;
        border: 1px solid currentColor;
    }
}
</style>
