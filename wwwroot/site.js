function downloadFile(base64, contentType, fileName) {
    const linkSource = `data:${contentType};base64,${base64}`;
    const downloadLink = document.createElement("a");
    downloadLink.href = linkSource;
    downloadLink.download = fileName;
    downloadLink.click();
}