document.addEventListener('DOMContentLoaded', function () {
    document.body.addEventListener('click', function (event) {
        const toggleButton = event.target.closest('.switch');

        if (toggleButton) {
            const switchOn = toggleButton.classList.contains('on') ? toggleButton : toggleButton.parentElement.querySelector('.on');
            const switchOff = toggleButton.classList.contains('off') ? toggleButton : toggleButton.parentElement.querySelector('.off');

            if (switchOn && switchOff) {
                if (switchOn.classList.contains('active')) {
                    switchOn.classList.remove('active');
                    switchOff.classList.add('active');
                } else {
                    switchOn.classList.add('active');
                    switchOff.classList.remove('active');
                }
            }
        }
    });
});

document.addEventListener('DOMContentLoaded', function () {
    const unfollowButtons = document.querySelectorAll('.unfollow');

    unfollowButtons.forEach(button => {
        const textSpan = button.querySelector('span');

        button.addEventListener('mouseover', function () {
            textSpan.textContent = 'Unfollow';
        });

        button.addEventListener('mouseout', function () {
            textSpan.textContent = 'Following';
        });
    });
});

document.addEventListener('DOMContentLoaded', function () {
    const elements = document.querySelectorAll('.element');

    elements.forEach(element => {
        element.addEventListener('click', function () {

            elements.forEach(el => el.querySelector('.text').classList.remove('active'));
            this.querySelector('.text').classList.add('active');
        });
    });
});

document.addEventListener('DOMContentLoaded', function () {
    const textareas = document.querySelectorAll('textarea');

    textareas.forEach(textarea => {
        textarea.addEventListener('input', function () {
            const computedStyle = getComputedStyle(this);
            const maxHeight = parseInt(computedStyle.maxHeight);

            this.style.height = 'auto';
            if (this.scrollHeight <= maxHeight) {
                this.style.height = this.scrollHeight + 'px';
                this.style.overflowY = 'hidden';
            } else {
                this.style.height = maxHeight + 'px';
                this.style.overflowY = 'auto';
            }
        });

        textarea.dispatchEvent(new Event('input'));
    });
});

document.addEventListener('DOMContentLoaded', function () {
    const scrollButton = document.querySelector('.scroll-button');
    const scrollContainer = scrollButton.parentElement;
    const scrollAmount = 200; // Adjust this value as needed

    scrollButton.addEventListener('click', function () {
        scrollContainer.scrollBy({
            left: scrollAmount,
            behavior: 'smooth'
        });
    });
});

var swiper = new Swiper(".swiper", {
    slidesPerView: 'auto',
    slidesPerGroup: 5,
    slidesOffsetAfter:15,
    loop: false,
    autoHeight: false,
    centeredSlides: false,
    allowTouchMove: false,
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev'
    },

});



