function switchPhoto(thumbEl, src) {
    const mainPhoto = document.getElementById('mainPhoto');
    if (!mainPhoto) return;

    mainPhoto.src = src;

    document.querySelectorAll('.gallery-thumb')
        .forEach(function (t) { t.classList.remove('active'); });

    thumbEl.classList.add('active');
}

function changeQty(delta) {
    const inp = document.getElementById('qtyInput');
    if (!inp) return;

    const max = parseInt(inp.max) || 99;
    let val = parseInt(inp.value) + delta;

    if (val < 1) val = 1;
    if (val > max) val = max;

    inp.value = val;
}