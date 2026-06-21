

(function () {
    'use strict';

    const body = document.body;
    const sidebar = document.getElementById('admSidebar');
    const toggleBtn = document.getElementById('sidebarToggleBtn');
    const closeBtn = document.getElementById('sidebarCloseBtn');
    const overlay = document.getElementById('sidebarOverlay');

    const COLLAPSE_KEY = 'adm_sidebar_collapsed';

    function isMobile() { return window.innerWidth < 992; }

    if (!isMobile() && localStorage.getItem(COLLAPSE_KEY) === '1') {
        body.classList.add('sidebar-collapsed');
    }

    function toggleSidebar() {
        if (isMobile()) {
            sidebar.classList.toggle('mobile-open');
            overlay.classList.toggle('show');
        } else {
            const collapsed = body.classList.toggle('sidebar-collapsed');
            localStorage.setItem(COLLAPSE_KEY, collapsed ? '1' : '0');
        }
    }

    function closeMobileSidebar() {
        sidebar && sidebar.classList.remove('mobile-open');
        overlay && overlay.classList.remove('show');
    }

    if (toggleBtn) toggleBtn.addEventListener('click', toggleSidebar);
    if (closeBtn) closeBtn.addEventListener('click', closeMobileSidebar);
    if (overlay) overlay.addEventListener('click', closeMobileSidebar);

    window.addEventListener('resize', function () {
        if (!isMobile()) closeMobileSidebar();
    });

    document.addEventListener('click', function (e) {
        const btn = e.target.closest('[data-confirm]');
        if (!btn) return;
        const msg = btn.getAttribute('data-confirm') || 'Bạn có chắc chắn không?';
        if (!window.confirm(msg)) {
            e.preventDefault();
            e.stopPropagation();
        }
    });

    document.addEventListener('change', function (e) {
        const input = e.target;
        if (input.type !== 'file') return;

        const previewSel = input.getAttribute('data-preview');
        if (!previewSel) return;

        const preview = document.querySelector(previewSel);
        if (!preview) return;

        const file = input.files && input.files[0];
        if (!file || !file.type.startsWith('image/')) return;

        const reader = new FileReader();
        reader.onload = function (ev) {
            preview.src = ev.target.result;
            preview.classList.remove('d-none');

            const hideSel = input.getAttribute('data-preview-hide');
            if (hideSel) {
                const ph = document.querySelector(hideSel);
                if (ph) ph.classList.add('d-none');
            }
        };
        reader.readAsDataURL(file);
    });

    document.querySelectorAll('.adm-alert.alert-dismissible').forEach(function (el) {
        setTimeout(function () {
            const bsAlert = window.bootstrap && bootstrap.Alert.getOrCreateInstance(el);
            if (bsAlert) bsAlert.close();
            else el.remove();
        }, 4000);
    });

    document.querySelectorAll('table.adm-table thead.adm-sortable th[data-col]').forEach(function (th) {
        th.style.cursor = 'pointer';
        th.addEventListener('click', function () {
            const table = th.closest('table');
            const tbody = table.querySelector('tbody');
            if (!tbody) return;

            const col = parseInt(th.getAttribute('data-col'), 10);
            const asc = th.getAttribute('data-order') !== 'asc';
            th.setAttribute('data-order', asc ? 'asc' : 'desc');

            table.querySelectorAll('th[data-col]').forEach(function (t) {
                if (t !== th) t.removeAttribute('data-order');
            });

            const rows = Array.from(tbody.querySelectorAll('tr'));
            rows.sort(function (a, b) {
                const aText = (a.cells[col] ? a.cells[col].innerText : '').trim();
                const bText = (b.cells[col] ? b.cells[col].innerText : '').trim();
                const aNum = parseFloat(aText.replace(/[^\d.-]/g, ''));
                const bNum = parseFloat(bText.replace(/[^\d.-]/g, ''));
                if (!isNaN(aNum) && !isNaN(bNum)) return asc ? aNum - bNum : bNum - aNum;
                return asc
                    ? aText.localeCompare(bText, 'vi')
                    : bText.localeCompare(aText, 'vi');
            });
            rows.forEach(function (row) { tbody.appendChild(row); });
        });
    });

    if (window.bootstrap && bootstrap.Tooltip) {
        document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function (el) {
            new bootstrap.Tooltip(el, { trigger: 'hover' });
        });
    }

    (function autoSlug() {
        const nameInput = document.getElementById('inputName')
            || document.getElementById('nameInput')
            || document.getElementById('titleInput')
            || document.querySelector('input[name="Name"]:not([type="hidden"])');

        const slugInput = document.getElementById('inputSlug')
            || document.getElementById('slugInput');

        if (!nameInput || !slugInput) return;

        nameInput.addEventListener('input', function () {
            if (slugInput.dataset.manual === '1') return;
            if (slugInput.value.trim() && slugInput.dataset.editMode === '1') return;
            slugInput.value = toSlug(nameInput.value);
        });

        slugInput.addEventListener('input', function () {
            this.dataset.manual = this.value ? '1' : '';
        });
    })();

})();

(function initNotifications() {
    const badge = document.getElementById('notifBadge');
    const list = document.getElementById('notifList');
    const toggleBtn = document.getElementById('notifToggleBtn');
    if (!badge || !list) return;

    async function fetchNotifications() {
        try {
            const res = await fetch('/Admin/Dashboard/GetNotifications');
            const data = await res.json();

            if (data.count > 0) {
                badge.textContent = data.count > 99 ? '99+' : data.count;
                badge.classList.remove('d-none');
            } else {
                badge.classList.add('d-none');
            }

            if (data.orders && data.orders.length > 0) {
                list.innerHTML = data.orders.map(function (o) {
                    return `<li class="border-bottom">
                            <a href="/Admin/Order/Detail/${o.id}"
                               class="dropdown-item py-2 px-3 text-decoration-none">
                                <div class="d-flex justify-content-between">
                                    <span class="fw-semibold text-warning small">${o.orderCode}</span>
                                    <span class="text-muted" style="font-size:.75rem">${o.createdAt}</span>
                                </div>
                                <div class="d-flex justify-content-between mt-1">
                                    <span class="small text-dark">${o.customerName}</span>
                                    <span class="small text-success fw-semibold">${o.total}</span>
                                </div>
                            </a>
                        </li>`;
                }).join('');
            } else {
                list.innerHTML = '<li class="text-muted text-center small py-3">Không có đơn hàng mới</li>';
            }
        } catch (e) {
            list.innerHTML = '<li class="text-danger text-center small py-3">Lỗi tải thông báo</li>';
        }
    }

    fetchNotifications();
    setInterval(fetchNotifications, 60_000);
    if (toggleBtn) toggleBtn.addEventListener('click', fetchNotifications);
})();

function previewImg(input, previewId, placeholderId) {
    var preview = document.getElementById(previewId);
    if (!preview || !input.files || !input.files[0]) return;

    var reader = new FileReader();
    reader.onload = function (e) {
        preview.src = e.target.result;
        preview.classList.remove('d-none');
        if (placeholderId) {
            var ph = document.getElementById(placeholderId);
            if (ph) ph.classList.add('d-none');
        }
    };
    reader.readAsDataURL(input.files[0]);
}

function toSlug(str) {
    return (str || '')
        .normalize('NFD')
        .replace(/[\u0300-\u036f]/g, '')
        .replace(/đ/gi, 'd')
        .toLowerCase()
        .replace(/[^a-z0-9\s-]/g, '')
        .trim()
        .replace(/\s+/g, '-')
        .replace(/-+/g, '-');
}