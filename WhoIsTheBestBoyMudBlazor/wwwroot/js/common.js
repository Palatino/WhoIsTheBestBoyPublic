function startCroppie(urlLink, format) {
    var el = document.getElementById('resizer-demo');

    var resize = new Croppie(el, {
        viewport: { width: 300, height: 300 },
        boundary: { width: 300, height: 300 },
        showZoomer: true,
        enableOrientation: true,
        mouseWheelZoom: 'ctrl'
    });
    resize.bind({
        url: `data:image/${format};base64,` + urlLink,
    });

    return resize;

}
