window.simuladorInterop = {
    drawFrame: function (canvasId, stateJson) {
        const canvas = document.getElementById(canvasId);
        if (!canvas) return;
        const ctx = canvas.getContext('2d');
        const state = JSON.parse(stateJson);
        const corpos = state.corpos || [];

        // ajustar tamanho se necessário (mas o Blazor ajustará também)
        // limpar
        ctx.fillStyle = '#0a0c25';
        ctx.fillRect(0, 0, canvas.width, canvas.height);

        // desenhar corpos
        corpos.forEach(c => {
            const raio = c.raio || (c.massa ? Math.cbrt((3 * (c.massa / c.densidade)) / (4 * Math.PI)) * 3 : 8);
            // radial gradient
            const gx = c.posX - raio / 3;
            const gy = c.posY - raio / 3;
            const g = ctx.createRadialGradient(gx, gy, Math.max(2, raio / 10), c.posX, c.posY, raio);
            g.addColorStop(0, 'rgba(255,255,255,0.8)');
            g.addColorStop(1, c.cor || 'hsl(200,80%,60%)');

            ctx.beginPath();
            ctx.arc(c.posX, c.posY, raio, 0, Math.PI * 2);
            ctx.fillStyle = g;
            ctx.fill();

            // brilho
            ctx.beginPath();
            ctx.arc(c.posX - raio / 3, c.posY - raio / 3, Math.max(1, raio / 4), 0, Math.PI * 2);
            ctx.fillStyle = 'rgba(255,255,255,0.35)';
            ctx.fill();

            // nome
            ctx.fillStyle = 'rgba(255,255,255,0.8)';
            ctx.font = '10px Arial';
            ctx.fillText(c.nome, c.posX - 15, c.posY + 4);
        });

        // stats
        ctx.fillStyle = '#a0a0ff';
        ctx.font = '12px Arial';
        const stats = `Corpos: ${state.corpos.length} | Iterações: ${state.iteracoes ?? 0} | Colisões: ${state.colisoes ?? 0} | FPS: ${state.fps ?? 0}`;
        ctx.fillText(stats, 10, 20);
    },

    downloadFile: function (filename, content) {
        const blob = new Blob([content], { type: 'application/json' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename || 'universo.json';
        a.click();
        URL.revokeObjectURL(url);
    },

    readFile: function () {
        return new Promise((resolve, reject) => {
            const input = document.createElement('input');
            input.type = 'file';
            input.accept = '.json,.txt';
            input.onchange = (e) => {
                const file = e.target.files[0];
                const reader = new FileReader();
                reader.onload = ev => resolve(ev.target.result);
                reader.onerror = err => reject(err);
                reader.readAsText(file);
            };
            input.click();
        });
    },

    setCanvasSize: function (canvasId, width, height) {
        const c = document.getElementById(canvasId);
        if (!c) return;
        c.width = width;
        c.height = height;
    }
};
