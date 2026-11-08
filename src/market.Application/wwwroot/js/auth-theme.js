(() => {
    const root = document.documentElement;
    const storageKey = 'shelf.theme';
    const validThemes = ['light', 'dark'];

    const applyTheme = (theme, save = false) => {
        const selected = validThemes.includes(theme) ? theme : 'light';
        root.setAttribute('data-bs-theme', selected);

        if (save) {
            try { localStorage.setItem(storageKey, selected); } catch { }
        }

        const nextTheme = selected === 'dark' ? 'light' : 'dark';
        document.querySelectorAll('[data-theme-label]').forEach(label => {
            label.textContent = nextTheme === 'dark' ? 'Dark mode' : 'Light mode';
        });
        document.querySelectorAll('[data-theme-toggle]').forEach(button => {
            button.setAttribute('aria-label', `Switch to ${nextTheme} theme`);
            button.setAttribute('aria-pressed', String(selected === 'dark'));
        });
    };

    let savedTheme = 'light';
    try {
        const stored = localStorage.getItem(storageKey);
        if (validThemes.includes(stored)) savedTheme = stored;
    } catch { }
    applyTheme(savedTheme);
    document.addEventListener('DOMContentLoaded', () => applyTheme(root.getAttribute('data-bs-theme')));

    document.addEventListener('click', event => {
        if (!event.target.closest('[data-theme-toggle]')) return;
        applyTheme(root.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark', true);
    });
})();
