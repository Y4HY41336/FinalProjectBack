document.addEventListener('DOMContentLoaded', function () {
    const stickyDiv = document.querySelector('.sticky-div');
    const marginTopDiv = document.querySelector('.margin-top');
    let lastScrollTop = 0;
    let scrolledDown = false;
    let scrolledUp = false;
    let topOffset = 200;
    let minHeight = 1200;

    window.addEventListener('scroll', function () {
        let scrollTop = window.scrollY || document.documentElement.scrollTop;

        if (scrollTop > lastScrollTop) {
            stickyDiv.style.top = '-200px';
            stickyDiv.style.bottom = '';

            if (!scrolledDown) {
                marginTopDiv.style.marginTop = scrollTop + 'px';

                scrolledDown = true;
                scrolledUp = false;
            }
        } else {
            stickyDiv.style.top = '';
            stickyDiv.style.bottom = '-58px';

            if (!scrolledUp) {
                marginTopDiv.style.marginTop = (scrollTop - topOffset) + 'px';

                scrolledUp = true;
                scrolledDown = false;
            }
        }

        lastScrollTop = scrollTop <= 0 ? 0 : scrollTop;
    });


});
