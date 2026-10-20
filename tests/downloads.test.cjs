const assert = require('node:assert/strict');
const { readFileSync } = require('node:fs');
const { test } = require('node:test');
const { join } = require('node:path');
const vm = require('node:vm');

const source = readFileSync(
    join(__dirname, '../TP3Frontend/wwwroot/js/downloads.js'),
    'utf8');

function createHarness() {
    const state = { attached: false, clicked: false, removed: false, revoked: false };
    const link = {
        click() {
            assert.equal(state.attached, true);
            assert.equal(state.revoked, false);
            state.clicked = true;
        },
        remove() { state.removed = true; }
    };
    const window = {
        setTimeout(callback, delay) {
            state.cleanup = callback;
            state.delay = delay;
        }
    };
    vm.runInNewContext(source, {
        window, Uint8Array, Blob,
        document: {
            createElement: () => link,
            body: { appendChild() { state.attached = true; } }
        },
        URL: {
            createObjectURL(blob) { state.blob = blob; return 'blob:test'; },
            revokeObjectURL(url) { assert.equal(url, 'blob:test'); state.revoked = true; }
        }
    });
    return { state, link, download: window.downloadFile };
}

test('preserves binary bytes and releases the URL after download initiation', async () => {
    const { state, link, download } = createHarness();
    const bytes = new Uint8Array([37, 80, 68, 70, 45, 49, 46, 55, 10, 0, 255, 128]);
    download('ventas.pdf', 'application/pdf', bytes);
    assert.equal(link.download, 'ventas.pdf');
    assert.equal(state.blob.type, 'application/pdf');
    assert.deepEqual(new Uint8Array(await state.blob.arrayBuffer()), bytes);
    assert.equal(state.clicked, true);
    assert.equal(state.removed, true);
    assert.equal(state.revoked, false);
    assert.equal(state.delay, 60_000);
    state.cleanup();
    assert.equal(state.revoked, true);
});

test('rejects empty, text and non-PDF responses without downloading', () => {
    for (const content of [new Uint8Array(), 'base64-text', new TextEncoder().encode('{"error":"unauthorized"}')]) {
        const { state, download } = createHarness();
        assert.throws(() => download('ventas.pdf', 'application/pdf', content));
        assert.equal(state.clicked, false);
        assert.equal(state.blob, undefined);
    }
});
