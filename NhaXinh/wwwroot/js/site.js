

(function () {
    'use strict';

    var header = document.querySelector('.nx-header');
    if (header) {
        var onScroll = function () {
            header.classList.toggle('scrolled', window.scrollY > 10);
        };
        window.addEventListener('scroll', onScroll, { passive: true });
        onScroll();
    }

    document.querySelectorAll('.nx-notification.alert').forEach(function (el) {
        setTimeout(function () {
            var bsAlert = bootstrap && bootstrap.Alert
                ? bootstrap.Alert.getOrCreateInstance(el)
                : null;
            if (bsAlert) bsAlert.close();
            else el.remove();
        }, 4000);
    });


    var currentPath = window.location.pathname;
    document.querySelectorAll('.nx-nav__link, .nx-offcanvas__link').forEach(function (link) {
        var href = link.getAttribute('href') || '';
        if (!href || href === '#') return;
        if (href === '/' && currentPath === '/') {
            link.classList.add('active');
        } else if (href !== '/' && currentPath.startsWith(href)) {
            link.classList.add('active');
        }
    });

    var searchCollapse = document.getElementById('searchCollapse');
    if (searchCollapse) {
        searchCollapse.addEventListener('shown.bs.collapse', function () {
            var input = searchCollapse.querySelector('.nx-search-input');
            if (input) input.focus();
        });
    }


    document.querySelectorAll('.payment-radio').forEach(function (radio) {
        radio.addEventListener('change', function () {
            document.querySelectorAll('.payment-option')
                .forEach(function (el) { el.classList.remove('active'); });
            var wrap = this.closest('.payment-option');
            if (wrap) wrap.classList.add('active');
        });
    });

})();


function togglePassword(inputId, iconId) {
    inputId = inputId || 'passwordInput';
    iconId = iconId || 'eyeIcon';

    var input = document.getElementById(inputId);
    var icon = document.getElementById(iconId);
    if (!input || !icon) return;

    if (input.type === 'password') {
        input.type = 'text';
        icon.className = 'bi bi-eye-slash';
    } else {
        input.type = 'password';
        icon.className = 'bi bi-eye';
    }
}


function switchImage(btn, src) {
    var mainImg = document.getElementById('mainImg');
    if (mainImg) mainImg.src = src;

    document.querySelectorAll('.nx-gallery__thumb')
        .forEach(function (t) { t.classList.remove('active'); });
    if (btn) btn.classList.add('active');
}


function changeQty(delta) {
    var input = document.getElementById('qtyInput');
    if (!input) return;

    var max = parseInt(input.max) || 999;
    var min = parseInt(input.min) || 1;
    var newVal = parseInt(input.value) || 1;

    newVal = Math.min(Math.max(min, newVal + delta), max);
    input.value = newVal;
}

function previewAvatar(input) {
    if (!input.files || !input.files[0]) return;

    var reader = new FileReader();
    reader.onload = function (e) {
        var preview = document.getElementById('avatarPreview');
        if (preview) preview.src = e.target.result;
    };
    reader.readAsDataURL(input.files[0]);
}

function decreaseQty(btn) {
    var input = btn.closest('.qty-control').querySelector('.qty-input');
    if (parseInt(input.value) > 1) {
        input.value = parseInt(input.value) - 1;
        input.closest('form').submit();
    }
}

function increaseQty(btn) {
    var input = btn.closest('.qty-control').querySelector('.qty-input');
    input.value = parseInt(input.value) + 1;
    input.closest('form').submit();
}