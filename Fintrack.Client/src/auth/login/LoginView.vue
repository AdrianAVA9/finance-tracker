<template>
  <div class="login-page">
    <h2>Sign In</h2>
    <form @submit.prevent>
      <div class="form-group">
        <label>Email</label>
        <input type="email" placeholder="you@example.com" class="glass-input" />
      </div>
      <div class="form-group">
        <label>Password</label>
        <input type="password" placeholder="••••••••" class="glass-input" />
      </div>
      <button class="btn-submit">Login</button>
    </form>
    <p class="switch-auth">
      Don't have an account? <router-link to="/auth/register">Sign Up</router-link>
    </p>
  </div>
</template>

<style scoped lang="scss">
h2 {
  text-align: center;
  margin-bottom: 1.5rem;
}
.form-group {
  margin-bottom: 1rem;
  label {
    display: block;
    margin-bottom: 0.5rem;
    font-size: 0.9rem;
    color: #555;
  }
  input {
    width: 100%;
    padding: 0.75rem;
    border-radius: 0.5rem;
    border: 1px solid #ccc;
    background: rgba(255,255,255,0.5);
  }
}
.btn-submit {
  width: 100%;
  padding: 0.75rem;
  background: hsl(var(--color-primary));
  color: white;
  border: none;
  border-radius: 0.5rem;
  cursor: pointer;
  font-weight: bold;
  margin-top: 1rem;
}
.switch-auth {
  text-align: center;
  margin-top: 1rem;
  font-size: 0.9rem;
}
</style>
