const revealElements = document.querySelectorAll("[data-reveal]");
const spotlight = document.querySelector(".spotlight");

if (revealElements.length > 0) {
    const revealObserver = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
            if (entry.isIntersecting) {
                entry.target.classList.add("reveal-in");
                revealObserver.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.18,
        rootMargin: "0px 0px -40px 0px"
    });

    revealElements.forEach((element, index) => {
        element.style.transitionDelay = `${Math.min(index * 70, 420)}ms`;
        revealObserver.observe(element);
    });
}

if (spotlight && !window.matchMedia("(prefers-reduced-motion: reduce)").matches) {
    window.addEventListener("pointermove", (event) => {
        spotlight.style.left = `${event.clientX}px`;
        spotlight.style.top = `${event.clientY}px`;
    });
}

const currentPath = window.location.pathname.toLowerCase();
document.querySelectorAll(".site-nav .nav-link").forEach((link) => {
    const href = (link.getAttribute("href") || "").toLowerCase();
    const sameSection =
        (href.startsWith("/transaction") && currentPath.startsWith("/transaction")) ||
        (href.startsWith("/home") && currentPath.startsWith("/home"));

    if (href && (currentPath === href || sameSection)) {
        link.classList.add("active");
    }
});
