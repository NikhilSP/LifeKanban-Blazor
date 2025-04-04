// Save this as LifeKanban/wwwroot/ThemeToggle.js
window.themeManager = {
    toggleTheme: function() {
        const currentTheme = localStorage.getItem('theme') || 'dark';
        const newTheme = currentTheme === 'dark' ? 'claude' : 'dark';

        document.documentElement.setAttribute('data-theme', newTheme);
        localStorage.setItem('theme', newTheme);

        return newTheme; // Return the new theme to Blazor
    },

    initTheme: function() {
        const savedTheme = localStorage.getItem('theme') || 'dark';
        document.documentElement.setAttribute('data-theme', savedTheme);
        return savedTheme; // Return the current theme to Blazor
    }
};