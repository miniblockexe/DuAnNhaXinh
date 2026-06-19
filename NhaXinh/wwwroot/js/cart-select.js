(function () {
    'use strict';

    const fmt = n => new Intl.NumberFormat('vi-VN').format(n) + ' ₫';
    const checkAll = document.getElementById('checkAll');
    const checkoutBtn = document.getElementById('checkoutBtn');

    function updateSummary() {
        const allChecks = document.querySelectorAll('.item-check');
        const checks = document.querySelectorAll('.item-check:checked');
        let total = 0;
        checks.forEach(cb => total += parseFloat(cb.dataset.price));

        const countEl = document.getElementById('selectedCount');
        const totalEl = document.getElementById('selectedTotal');
        const grandEl = document.getElementById('grandTotal');

        if (countEl) countEl.textContent = checks.length;
        if (totalEl) totalEl.textContent = fmt(total);
        if (grandEl) grandEl.textContent = fmt(total);

        if (checkoutBtn) {
            const ids = Array.from(checks).map(cb =>
                cb.closest('.cart-item').dataset.productId);
            const query = ids.length > 0 ? '?selectedIds=' + ids.join(',') : '';
            checkoutBtn.href = '/Order/Checkout' + query;
        }

        if (checkAll) {
            checkAll.checked = allChecks.length > 0 && checks.length === allChecks.length;
            checkAll.indeterminate = checks.length > 0 && checks.length < allChecks.length;
        }
    }

    if (checkAll) {
        checkAll.addEventListener('change', function () {
            document.querySelectorAll('.item-check')
                .forEach(cb => cb.checked = this.checked);
            updateSummary();
        });
    }

    document.querySelectorAll('.item-check').forEach(cb => {
        cb.addEventListener('change', updateSummary);
    });

    updateSummary();
})();