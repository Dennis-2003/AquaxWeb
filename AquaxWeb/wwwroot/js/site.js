(function () {
    const container = document.getElementById('snackbar-container');
    if (!container) return;

    window.showSnackbar = function (message, type, duration) {
        type = type || 'success';
        duration = duration || 4000;

        const icons = {
            success: 'check_circle',
            error: 'error',
            info: 'info',
            warning: 'warning'
        };

        const bgColors = {
            success: 'bg-[#1b5e20]',
            error: 'bg-[#ba1a1a]',
            info: 'bg-[#001430]',
            warning: 'bg-[#e65100]'
        };

        const snackbar = document.createElement('div');
        snackbar.className = `flex items-center gap-3 px-4 py-3 rounded-xl shadow-2xl text-white text-sm font-medium ${bgColors[type] || bgColors.success} translate-y-4 opacity-0 transition-all duration-300`;
        snackbar.style.minWidth = '280px';
        snackbar.style.maxWidth = '480px';
        snackbar.innerHTML = `
            <span class="material-symbols-outlined" style="font-size:20px">${icons[type] || icons.success}</span>
            <span class="flex-1">${message}</span>
            <button onclick="this.closest('#snackbar-container > div').remove()" class="text-white/70 hover:text-white transition-colors">
                <span class="material-symbols-outlined" style="font-size:18px">close</span>
            </button>
        `;

        container.appendChild(snackbar);

        requestAnimationFrame(() => {
            snackbar.classList.remove('translate-y-4', 'opacity-0');
            snackbar.classList.add('translate-y-0', 'opacity-100');
        });

        setTimeout(() => {
            snackbar.classList.remove('translate-y-0', 'opacity-100');
            snackbar.classList.add('translate-y-4', 'opacity-0');
            setTimeout(() => snackbar.remove(), 300);
        }, duration);
    };

    const dataEl = document.getElementById('snackbar-data');
    if (dataEl) {
        const msg = dataEl.getAttribute('data-message');
        const type = dataEl.getAttribute('data-type') || 'success';
        if (msg) {
            setTimeout(() => showSnackbar(msg, type), 300);
        }
        dataEl.remove();
    }
})();
