function createParticle() {
    let container = $('.particle-container');

    if (container.children().length > 100) {
        return;
    }

    for (var i = 0; i < 25; i++) {
        let particle = document.createElement('div');
        particle.classList.add('particle');
        particle.style.top = Math.random() * 100 + '%';
        particle.style.left = Math.random() * 100 + '%';
        particle.style.width = Math.random() * 10 + 'px';
        particle.style.height = particle.style.width;
        container.append(particle);
    }
}

setInterval(createParticle, 500);