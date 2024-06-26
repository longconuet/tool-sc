var requestAnimationFrame = window.requestAnimationFrame || window.mozRequestAnimationFrame || window.webkitRequestAnimationFrame || window.msRequestAnimationFrame;
var transforms = ["transform", "msTransform", "webkitTransform", "mozTransform", "oTransform"];
var transformProperty = getSupportedPropertyName(transforms);
var suns = [];
var browserWidth;
var browserHeight;
var numberOfSuns = 7;
var numberOfCrabs = 5;
var resetPosition = false;

function setup() {
    window.addEventListener("DOMContentLoaded", generateSuns, false);
    window.addEventListener("DOMContentLoaded", generateCrabs, false);
    window.addEventListener("resize", setResetFlag, false)
}
setup();

function getSupportedPropertyName(b) {
    for (var a = 0; a < b.length; a++) {
        if (typeof document.body.style[b[a]] != "undefined") {
            return b[a]
        }
    }
    return null
}

function Sun(b, a, d, e, c) {
    this.element = b;
    this.radius = a;
    this.speed = d;
    this.xPos = e;
    this.yPos = c;
    this.counter = 0;
    this.sign = Math.random() < 0.5 ? 1 : -1;
    this.element.style.opacity = 0.5 + Math.random();
    this.element.style.fontSize = 4 + Math.random() * 30 + "px"
}
Sun.prototype.update = function () {
    this.counter += this.speed / 5000;
    this.xPos += this.sign * this.speed * Math.cos(this.counter) / 40;
    this.yPos += Math.sin(this.counter) / 40 + this.speed / 30;
    setTranslate3DTransform(this.element, Math.round(this.xPos), Math.round(this.yPos));
    if (this.yPos > browserHeight) {
        this.yPos = -50
    }
};

function setTranslate3DTransform(a, c, b) {
    var d = "translate3d(" + c + "px, " + b + "px, 0)";
    a.style[transformProperty] = d
}

function generateSuns() {
    var b = document.querySelector(".sun");
    var h = b.parentNode;
    browserWidth = document.documentElement.clientWidth;
    browserHeight = document.documentElement.clientHeight;
    for (var d = 0; d < numberOfSuns; d++) {
        var j = b.cloneNode(true);
        h.appendChild(j);
        var e = getPosition(50, browserWidth);
        var a = getPosition(50, browserHeight);
        var c = 5 + Math.random() * 30;
        var g = 4 + Math.random() * 50;
        var f = new Sun(j, g, c, e, a);
        suns.push(f)
    }
    h.removeChild(b);
    moveSuns()
}

function generateCrabs() {
    var b = document.querySelector(".crab");
    var h = b.parentNode;
    browserWidth = document.documentElement.clientWidth;
    browserHeight = document.documentElement.clientHeight;
    for (var d = 0; d < numberOfCrabs; d++) {
        var j = b.cloneNode(true);
        h.appendChild(j);
        var e = getPosition(50, browserWidth);
        var a = getPosition(50, browserHeight);
        var c = 5 + Math.random() * 40;
        var g = 4 + Math.random() * 50;
        var f = new Sun(j, g, c, e, a);
        suns.push(f)
    }
    h.removeChild(b);
    moveSuns()
}

function moveSuns() {
    for (var b = 0; b < suns.length; b++) {
        var a = suns[b];
        a.update()
    }
    if (resetPosition) {
        browserWidth = document.documentElement.clientWidth;
        browserHeight = document.documentElement.clientHeight;
        for (var b = 0; b < suns.length; b++) {
            var a = suns[b];
            a.xPos = getPosition(50, browserWidth);
            a.yPos = getPosition(50, browserHeight)
        }
        resetPosition = false
    }
    requestAnimationFrame(moveSuns)
}

function getPosition(b, a) {
    return Math.round(-1 * b + Math.random() * (a + 2 * b))
}

function setResetFlag(a) {
    resetPosition = true
};