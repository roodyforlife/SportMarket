function PrintAsImage(div, fileName) {
    html2canvas(document.querySelector(div)).then(canvas => {
        var dataURL = canvas.toDataURL("image/jpeg", 1.0);
        downloadImage(dataURL, `${fileName}.jpeg`);
    });
}

    function downloadImage(data, filename = 'untitled.jpeg') {
        var a = document.createElement('a');
        a.href = data;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
    }