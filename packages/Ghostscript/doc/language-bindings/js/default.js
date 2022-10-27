var showingBurgerMenu = false;

function showMenu() {
    showingBurgerMenu = !showingBurgerMenu;

    var mobileMenu = document.getElementById("burger-menu");

    if (showingBurgerMenu) {
        mobileMenu.classList.add("show");
    } else {
        mobileMenu.classList.remove("show");
    }

}

function copyText(buttonDiv) {
    for (var i=0;i<buttonDiv.parentNode.childNodes.length;i++) {

        if (buttonDiv.parentNode.childNodes[i].tagName!=undefined) {
            if (buttonDiv.parentNode.childNodes[i].tagName.toLowerCase()=="code") {
                var codeBlock = buttonDiv.parentNode.childNodes[i];
                var copyText = codeBlock.innerHTML;
                const textArea = document.createElement('textarea');
                textArea.textContent = copyText;
                document.body.append(textArea);
                textArea.select();
                document.execCommand("copy");
            }
        }
    }
}

function showCopyButton(preDiv) {
    for (var i=0;i<preDiv.childNodes.length;i++) {

        if (preDiv.childNodes[i].tagName!=undefined) {
            if (preDiv.childNodes[i].tagName.toLowerCase()=="button") {
                preDiv.childNodes[i].classList.add("show");
            }
        }
    }
}

function hideCopyButton(preDiv) {
    for (var i=0;i<preDiv.childNodes.length;i++) {

        if (preDiv.childNodes[i].tagName!=undefined) {
            if (preDiv.childNodes[i].tagName.toLowerCase()=="button") {
                preDiv.childNodes[i].classList.remove("show");
            }
        }
    }
}
