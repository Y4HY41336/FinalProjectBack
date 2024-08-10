$(document).ready(function () {
    initializeEventListeners();
});

function initializeEventListeners() {
    let Repost = document.querySelectorAll(".retweet-green");
    let Like = document.querySelectorAll(".like-red");
    let BookMark = document.querySelectorAll(".bookmark-blue");
    let BanButton = document.querySelectorAll(".ban-button");
    let FollowButton = document.querySelectorAll(".follow-unfollow");
    let FollowModal = document.querySelectorAll(".follow-modal");
    let PostContent = document.querySelector(".post-content.tab-pane");
    let Reply = document.querySelectorAll(".comment-blue");
    let ReplyModal = document.getElementById('ReplyModal-button');
    let textareas = document.querySelectorAll(".textarea");
    let DeletePost = document.querySelectorAll(".delete-post");
    let Posts = document.querySelectorAll(".post");

    let MainRepost = document.querySelectorAll(".main-retweet-green");
    let MainLike = document.querySelectorAll(".main-like-red");
    let MainBookMark = document.querySelectorAll(".main-bookmark-blue");
    let MainPost = document.querySelector(".mainpost-container");

    MainRepost.forEach(MainRepost => {
        MainRepost.addEventListener("click", function (e) {

            e.preventDefault();

            let url = this.getAttribute("href");

            fetch(url).then(response => response.text())
                .then(data => {
                    MainPost.innerHTML = data;
                    initializeEventListeners();
                });

        })
    })
    MainLike.forEach(MainLike => {
        MainLike.addEventListener("click", function (e) {

            e.preventDefault();

            let url = this.getAttribute("href");

            fetch(url).then(response => response.text())
                .then(data => {
                    MainPost.innerHTML = data;
                    initializeEventListeners();
                });

        })
    })
    MainBookMark.forEach(MainBookMark => {
        MainBookMark.addEventListener("click", function (e) {

            e.preventDefault();

            let url = this.getAttribute("href");

            fetch(url).then(response => response.text())
                .then(data => {
                    MainPost.innerHTML = data;
                    initializeEventListeners();
                });

        })
    })

    Repost.forEach(Repost => {
        Repost.addEventListener("click", function (e) {

            e.preventDefault();

            let url = this.getAttribute("href");

            fetch(url).then(response => response.text())
                .then(data => {
                    //Repost.innerHTML = data;
                    PostContent.innerHTML = data;
                    initializeEventListeners();
                });

        })
    })

    Like.forEach(Like => {
        Like.addEventListener("click", function (e) {

            e.preventDefault();

            let url = this.getAttribute("href");

            fetch(url).then(response => response.text())
                .then(data => {
                    //Like.innerHTML = data;
                    PostContent.innerHTML = data;

                    initializeEventListeners();
                });
        })
    })


    BookMark.forEach(BookMark => {
        BookMark.addEventListener("click", function (e) {

            e.preventDefault();

            let url = this.getAttribute("href");

            fetch(url).then(response => response.text())
                .then(data => {
                    /*BookMark.innerHTML = data;*/
                    PostContent.innerHTML = data;
                    initializeEventListeners();
                });
        })
    })

    Reply.forEach(Reply => {
        Reply.addEventListener("click", function (e) {

            e.preventDefault();

            let url = this.getAttribute("asp-route-postId");
            ReplyModal.setAttribute('asp-route-postId', url);
            ReplyModal.addEventListener("click", function (e) {
                ReplyModal.setAttribute('asp-route-postId', url);

            })

        })
    })

    FollowButton.forEach(FollowButton => {
        FollowButton.addEventListener("click", function (e) {

            e.preventDefault();

            let url = this.getAttribute("href");

            fetch(url)
        })
    })

    BanButton.forEach(BanButton => {
        BanButton.addEventListener("click", function (e) {

            e.preventDefault();

            let url = this.getAttribute("href");

            fetch(url)
        })
    })

    FollowModal.forEach(FollowButton => {
        FollowButton.addEventListener("click", function (e) {

            e.preventDefault();

            let url = this.getAttribute("href");

            fetch(url)
        })
    })

    DeletePost.forEach(DeletePost => {
        DeletePost.addEventListener("click", function (e) {

            e.preventDefault();

            let url = this.getAttribute("href");

            fetch(url).then(response => response.text())
                .then(data => {
                    PostContent.innerHTML = data;
                    initializeEventListeners();
                });
        })
    })

    textareas.forEach(textarea => {
        let postButton = textarea.closest(".post-area").querySelector(".post-submit");
        textarea.addEventListener('input', function () {
            if (textarea.value.trim().length > 0) {
                postButton.disabled = false;

            } else {
                postButton.disabled = true;
            }
        });
    });

    Posts.forEach(Post => {
        let CustomUser = Post.querySelector(".post-customuser");
        let Likes = Post.querySelectorAll("a");
        let Items = Post.querySelectorAll(".more");
        let isClicked = false
        let Modal = Post.querySelector(".modal");

        Likes.forEach(Like => {
            Like.addEventListener('click', function (e) {
                isClicked = true

            })
        })

        Items.forEach(Item => {
            Item.addEventListener('click', function (e) {
                isClicked = true

            })
        })

        if (Modal != null) {
            Modal.addEventListener('click', function (e) {
                isClicked = true


            })
        }

        Post.addEventListener('click', function (e) {
            if (!isClicked) {
                url = CustomUser.getAttribute("href");
                window.location.href = url;
                initializeEventListeners();
            }
            else {
                isClicked = false
            }

        });
    });

    let FollowList = document.querySelectorAll(".list-follow");
    let UnfollowList = document.querySelectorAll(".list-unfollow");
    let Content = document.querySelector(".list-content");

    FollowList.forEach(FollowList => {
        FollowList.addEventListener("click", function (e) {
            e.preventDefault();

            let url = this.getAttribute("href");

            fetch(url).then(response => response.text())
                .then(data => {
                    Content.innerHTML = data;
                    console.log(1)
                    initializeEventListeners();
                });
        })
    })

    UnfollowList.forEach(UnfollowList => {
        UnfollowList.addEventListener("click", function (e) {
            e.preventDefault();

            let url = this.getAttribute("href");

            fetch(url).then(response => response.text())
                .then(data => {
                    Content.innerHTML = data;
                    console.log(1)
                    initializeEventListeners();
                });
        })
    })

    $('.element').off('click').on("click", function (e) {
        e.preventDefault();
        let Url = this.getAttribute("href");
        $.ajax({
            type: 'GET',
            url: Url,
            success: function (result) {
                $(".post-content.tab-pane").html(result);
                $(".Foryou-content.tab-pane").html(result);
                $(".tab-content").html(result);
                initializeEventListeners();
            },
            error: function () {
                console.error('Error');
            }
        });
    });


    let backButton = document.getElementById('backButton');

    if (backButton != null) {
        backButton.addEventListener('click', function () {
            window.history.back()
        });
    }



}


