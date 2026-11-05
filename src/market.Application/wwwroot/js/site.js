(() => {
    const root = document.documentElement;
    if (!root.hasAttribute('data-market-theme')) return;
    const themeKey = 'poshakghor-theme';
    const themes = ['clearance', 'new-collections'];

    const setTheme = (theme, persist = true) => {
        const selected = themes.includes(theme) ? theme : 'clearance';
        root.dataset.marketTheme = selected;
        root.dataset.bsTheme = selected === 'clearance' ? 'dark' : 'light';
        if (persist) {
            try {
                localStorage.setItem(themeKey, selected);
                localStorage.setItem('templateCustomizer-marketplace--Theme', selected === 'clearance' ? 'dark' : 'light');
            } catch { /* Storage may be disabled; the current page still updates. */ }
        }
        const nextName = selected === 'clearance' ? 'New Collections' : 'Clearance Market';
        document.querySelectorAll('[data-theme-toggle]').forEach(button => {
            button.setAttribute('aria-label', `Switch to ${nextName} theme`);
            button.title = `Switch to ${nextName} theme`;
        });
        document.querySelectorAll('[data-theme-label]').forEach(label => { label.textContent = nextName; });
        document.querySelectorAll('[data-theme-option]').forEach(option => {
            const active = option.dataset.themeOption === selected;
            option.setAttribute('aria-pressed', String(active));
            option.classList.toggle('active', active);
        });
        document.dispatchEvent(new CustomEvent('poshakghor:themechange', { detail: { theme: selected } }));
    };

    try { setTheme(localStorage.getItem(themeKey) || root.dataset.marketTheme, false); }
    catch { setTheme(root.dataset.marketTheme, false); }

    document.addEventListener('click', event => {
        const toggle = event.target.closest('[data-theme-toggle]');
        if (toggle) {
            setTheme(root.dataset.marketTheme === 'clearance' ? 'new-collections' : 'clearance');
            return;
        }
        const option = event.target.closest('[data-theme-option]');
        if (option) setTheme(option.dataset.themeOption);

        const quantityButton = event.target.closest('[data-quantity-step]');
        if (quantityButton) {
            const field = quantityButton.closest('.pg-quantity')?.querySelector('input');
            if (field) {
                const minimum = Number(field.min || 1);
                const maximum = Number(field.max || 999);
                field.value = String(Math.max(minimum, Math.min(maximum, Number(field.value || minimum) + Number(quantityButton.dataset.quantityStep))));
                field.dispatchEvent(new Event('change', { bubbles: true }));
            }
        }

        const loadButton = event.target.closest('[data-demo-loading]');
        if (loadButton && !loadButton.disabled) {
            const original = loadButton.innerHTML;
            loadButton.disabled = true;
            loadButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>Loading';
            window.setTimeout(() => { loadButton.disabled = false; loadButton.innerHTML = original; }, 1200);
        }

        const toastTrigger = event.target.closest('[data-demo-toast]');
        if (toastTrigger && window.bootstrap?.Toast) {
            const toast = document.getElementById('designSystemToast');
            if (toast) window.bootstrap.Toast.getOrCreateInstance(toast).show();
        }
    });

    document.addEventListener('submit', event => {
        if (event.target.matches('[data-product-search]')) {
            event.preventDefault();
            const query = event.target.querySelector('input')?.value.trim().toLocaleLowerCase() || '';
            const items = document.querySelectorAll('[data-product-item]');
            let visible = 0;
            items.forEach(item => {
                const matches = !query || item.dataset.searchText.includes(query);
                item.hidden = !matches;
                if (matches) visible += 1;
            });
            const feedback = document.querySelector('[data-search-feedback]');
            if (feedback) feedback.textContent = query ? `${visible} ${visible === 1 ? 'piece' : 'pieces'} found for “${event.target.querySelector('input').value.trim()}”.` : '';
            return;
        }
        if (!event.target.matches('[data-demo-validation]')) return;
        event.preventDefault();
        event.target.classList.add('was-validated');
    });
})();
