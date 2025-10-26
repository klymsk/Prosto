document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.querySelector('.searchInput');
    const items = document.querySelectorAll('.item');

    if (!searchInput || items.length === 0) {
        console.warn('Search input or items not found');
        return;
    }

    searchInput.addEventListener('input', () => {
        const query = searchInput.value.toLowerCase().trim();

        items.forEach(item => {
            // Отримуємо назву товару всередині поточного item
            const nameElement = item.querySelector('p');
            const name = nameElement ? nameElement.textContent.toLowerCase() : '';

            // Показуємо/ховаємо залежно від збігу
            item.closest('a').style.display = name.includes(query) ? '' : 'none';
        });
    });
});