function generateQRCode(elementId, text, size) {
    const element = document.getElementById(elementId);
    if (!element) return;

    element.innerHTML = ""; // Clear existing QR code if any
    new QRCode(element, {
        text: text,
        width: size,
        height: size,
    });
}