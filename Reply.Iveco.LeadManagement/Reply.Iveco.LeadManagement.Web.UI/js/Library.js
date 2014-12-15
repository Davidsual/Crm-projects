var modal = document.getElementById('modal');
var shade = document.getElementById('shade');
//            document.getElementById('start').onclick = function() {
//                modal.style.display = shade.style.display = 'block';
//            };
//            document.getElementById('close').onclick = function() {
//                modal.style.display = shade.style.display = 'none';
//                return false;
//            };

// This code is a workaround for IE6's lack of support for the 
// position: fixed style. 
// 
if (!('maxHeight' in document.body.style)) {
    function modalsize() {
        var top = document.documentElement.scrollTop;
        var winsize = document.documentElement.offsetHeight;
        var docsize = document.documentElement.scrollHeight;
        shade.style.height = Math.max(winsize, docsize) + 'px';
        modal.style.top = top + Math.floor(winsize / 3) + 'px';
    };
    modal.style.position = shade.style.position = 'absolute';
    window.onscroll = window.onresize = modalsize;
    modalsize();
}

function OperationComplete() {
    modal.style.display = shade.style.display = 'block'; return false;
}
function HidePanel() {
    modal.style.display = shade.style.display = 'none';
}     