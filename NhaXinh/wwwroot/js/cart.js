
(function () {
    'use strict';

    const fmt = (window.NhaXinh && window.NhaXinh.formatCurrency)
        || function (n) { return new Intl.NumberFormat('vi-VN').format(n) + ' ₫'; };

    const toast = (window.NhaXinh && window.NhaXinh.toast)
        || function (msg) { alert(msg); };

    const cartTable = document.getElementById('cartTable');

    if (cartTable) {

        cartTable.addEventListener('click', function (e) {
            const btn = e.target.closest('.cart-qty-btn');
            if (!btn) return;

            const row = btn.closest('tr[data-item-id]');
            if (!row) return;

            const itemId = row.dataset.itemId;
            const input = row.querySelector('.cart-qty-input');
            const action = btn.dataset.action;

            let qty = parseInt(input.value, 10) || 1;
            if (action === 'inc') qty += 1;
            if (action === 'dec') qty = Math.max(1, qty - 1);

            input.value = qty;
            updateCartItem(itemId, qty, row);
        });

        cartTable.addEventListener('change', function (e) {
            const input = e.target.closest('.cart-qty-input');
            if (!input) return;

            const row = input.closest('tr[data-item-id]');
            if (!row) return;

            let qty = parseInt(input.value, 10) || 1;
            qty = Math.max(1, qty);
            input.value = qty;
            updateCartItem(row.dataset.itemId, qty, row);
        });

        cartTable.addEventListener('click', function (e) {
            const btn = e.target.closest('.cart-remove-btn');
            if (!btn) return;

            const row = btn.closest('tr[data-item-id]');
            if (!row) return;

            const name = row.querySelector('.cart-item-name')?.textContent?.trim() || 'sản phẩm';
            if (!confirm('Xoá "' + name + '" khỏi giỏ hàng?')) return;

            removeCartItem(row.dataset.itemId, row);
        });

        function updateCartItem(itemId, qty, row) {
            setRowLoading(row, true);

            fetch('/Cart/UpdateQuantity', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken()
                },
                body: JSON.stringify({ itemId: itemId, quantity: qty })
            })
                .then(function (res) { return res.json(); })
                .then(function (data) {
                    if (data.success) {
                        const lineTotal = row.querySelector('.cart-line-total');
                        if (lineTotal && data.lineTotal != null) {
                            lineTotal.textContent = fmt(data.lineTotal);
                        }
                        updateCartSummary(data);
                        updateHeaderCartCount(data.cartCount);
                    } else {
                        toast(data.message || 'Không thể cập nhật.', 'error');
                        if (data.currentQty != null) {
                            const input = row.querySelector('.cart-qty-input');
                            if (input) input.value = data.currentQty;
                        }
                    }
                })
                .catch(function () {
                    toast('Lỗi kết nối. Vui lòng thử lại.', 'error');
                })
                .finally(function () {
                    setRowLoading(row, false);
                });
        }

        function removeCartItem(itemId, row) {
            setRowLoading(row, true);

            fetch('/Cart/RemoveItem', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken()
                },
                body: JSON.stringify({ itemId: itemId })
            })
                .then(function (res) { return res.json(); })
                .then(function (data) {
                    if (data.success) {
                        row.style.transition = 'opacity .3s, transform .3s';
                        row.style.opacity = '0';
                        row.style.transform = 'translateX(20px)';
                        setTimeout(function () {
                            row.remove();
                            updateCartSummary(data);
                            updateHeaderCartCount(data.cartCount);
                            checkCartEmpty();
                        }, 300);

                        toast('Đã xoá khỏi giỏ hàng.', 'success');
                    } else {
                        toast(data.message || 'Không thể xoá.', 'error');
                        setRowLoading(row, false);
                    }
                })
                .catch(function () {
                    toast('Lỗi kết nối. Vui lòng thử lại.', 'error');
                    setRowLoading(row, false);
                });
        }

        function updateCartSummary(data) {
            const subtotalEl = document.getElementById('cartSubtotal');
            const shippingEl = document.getElementById('cartShipping');
            const totalEl = document.getElementById('cartTotal');
            const countEl = document.getElementById('cartItemCount');

            if (subtotalEl && data.subtotal != null) subtotalEl.textContent = fmt(data.subtotal);
            if (shippingEl && data.shipping != null) shippingEl.textContent = data.shipping === 0 ? 'Miễn phí' : fmt(data.shipping);
            if (totalEl && data.total != null) totalEl.textContent = fmt(data.total);
            if (countEl && data.cartCount != null) countEl.textContent = data.cartCount + ' sản phẩm';
        }

        function checkCartEmpty() {
            const rows = cartTable.querySelectorAll('tbody tr[data-item-id]');
            if (rows.length === 0) {
                const emptyEl = document.getElementById('cartEmpty');
                const cartBox = document.getElementById('cartBox');
                if (emptyEl) emptyEl.style.display = 'block';
                if (cartBox) cartBox.style.display = 'none';
            }
        }

        function setRowLoading(row, loading) {
            row.style.opacity = loading ? '0.5' : '1';
            row.style.pointerEvents = loading ? 'none' : '';
        }
    }

    document.addEventListener('submit', function (e) {
        const form = e.target.closest('form[data-cart-add]');
        if (!form) return;
        e.preventDefault();

        const btn = e.submitter || form.querySelector('button[type="submit"]');
        const origHTML = btn ? btn.innerHTML : '';
        if (btn) { btn.disabled = true; btn.innerHTML = '<span class="spinner-border spinner-border-sm"></span>'; }

        const fd = new FormData(form);
        if (e.submitter && e.submitter.name) {
            fd.set(e.submitter.name, e.submitter.value);
        }

        fetch(form.action, {
            method: 'POST',
            body: fd
        })
            .then(function (res) { return res.json(); })
            .then(function (data) {
                if (data.success) {
                    if (data.redirectUrl) {
                        window.location.href = data.redirectUrl;
                        return;
                    }
                    updateHeaderCartCount(data.cartCount);
                } else {
                    toast(data.message || 'Không thể thêm vào giỏ.', 'error');
                }
            })
            .catch(function () {
                toast('Lỗi kết nối. Vui lòng thử lại.', 'error');
            })
            .finally(function () {
                if (btn) { btn.disabled = false; btn.innerHTML = origHTML; }
            });
    });

    function updateHeaderCartCount(count) {
        const badge = document.querySelector('.nx-cart-btn .nx-badge');
        if (count == null) return;

        if (count > 0) {
            if (badge) {
                badge.textContent = count;
            } else {
                const cartBtn = document.querySelector('.nx-cart-btn');
                if (cartBtn) {
                    const span = document.createElement('span');
                    span.className = 'nx-badge';
                    span.textContent = count;
                    cartBtn.appendChild(span);
                }
            }
        } else {
            if (badge) badge.remove();
        }
    }

    const checkoutForm = document.getElementById('checkoutForm');
    if (checkoutForm) {
        checkoutForm.addEventListener('submit', function (e) {
            let valid = true;
            checkoutForm.querySelectorAll('[required]').forEach(function (field) {
                const err = field.parentElement.querySelector('.nx-field-error');
                if (!field.value.trim()) {
                    valid = false;
                    field.classList.add('nx-form-control--invalid');
                    if (err) err.textContent = 'Vui lòng điền thông tin này.';
                } else {
                    field.classList.remove('nx-form-control--invalid');
                    if (err) err.textContent = '';
                }
            });

            if (!valid) {
                e.preventDefault();
                const firstErr = checkoutForm.querySelector('.nx-form-control--invalid');
                if (firstErr) firstErr.scrollIntoView({ behavior: 'smooth', block: 'center' });
                toast('Vui lòng điền đầy đủ thông tin.', 'error');
            } else {
                const submitBtn = checkoutForm.querySelector('[type="submit"]');
                if (submitBtn) {
                    submitBtn.disabled = true;
                    submitBtn.textContent = 'Đang xử lý...';
                }
            }
        });

        checkoutForm.addEventListener('input', function (e) {
            const field = e.target;
            if (field.classList.contains('nx-form-control--invalid') && field.value.trim()) {
                field.classList.remove('nx-form-control--invalid');
                const err = field.parentElement.querySelector('.nx-field-error');
                if (err) err.textContent = '';
            }
        });
    }

    function getAntiForgeryToken() {
        const input = document.querySelector('input[name="__RequestVerificationToken"]');
        return input ? input.value : '';
    }

})();