// Blazor passes byte[] as Uint8Array. Keep the URL alive until the browser
// has had time to consume the download, then release the temporary resource.
window.downloadFile = (fileName, contentType, content) => {
    if (!(content instanceof Uint8Array) || content.length === 0) {
        throw new Error("El archivo recibido está vacío o tiene un formato inválido.");
    }

    if (contentType === "application/pdf" &&
        String.fromCharCode(...content.subarray(0, 5)) !== "%PDF-") {
        throw new Error("La respuesta recibida no es un archivo PDF válido.");
    }

    const blob = new Blob([content], { type: contentType });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");

    try {
        link.href = url;
        link.download = fileName;
        link.hidden = true;
        document.body.appendChild(link);
        link.click();
    } finally {
        link.remove();
        window.setTimeout(() => URL.revokeObjectURL(url), 60_000);
    }
};
