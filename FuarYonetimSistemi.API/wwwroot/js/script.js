// js/script.js (Dinamik menü, yönlendirme ve orijinal tema işlevselliği ile)

document.addEventListener('DOMContentLoaded', async () => {
    const path = window.location.pathname.toLowerCase();

    // Login sayfası dışındaki tüm sayfalar için token kontrolü ve UI oluşturma
    if (!path.includes("login.html")) {
        if (!localStorage.getItem('token')) {
            window.location.href = 'login.html';
            return;
        }

        // Dinamik olarak Sidebar ve Topbar bileşenlerini oluştur
        if (typeof window.buildSidebar === "function") window.buildSidebar();
        if (typeof window.buildTopbar === "function") window.buildTopbar();

        // Bildirim sayılarını çek (asenkron olarak arka planda çalışır)
        if (typeof window.updateTopbarCounters === "function") window.updateTopbarCounters();

        // Sayfaya özel JS fonksiyonunu çalıştır
        await routePage(path);
    }
});

/**
 * Yüklü olan sayfanın yoluna göre ilgili JS fonksiyonunu çağırır.
 * @param {string} path - Mevcut sayfanın yolu.
 */
async function routePage(path) {
    const params = new URLSearchParams(window.location.search);
    const pageLoaders = {
        "home.html": window.loadHomePage,
        "listfuar.html": window.loadFairsPage,
        "listparticipant.html": window.loadParticipantsPage,
        "liststand.html": window.loadStandsPage,
        "listfairexpense.html": window.loadFairExpensesPage,
        "listofficeexpenses.html": window.loadOfficeExpensesPage,
        "listpayment.html": window.loadPaymentsPage,
        "listuser.html": window.loadUsersPage,
        "listlogs.html": window.loadLogsPage,
        "listtargets.html": window.loadTargetsPage,
        "sharedfiles.html": window.loadSharedFilesPage,
        "messaging.html": window.loadMessagingPage,
        "fairdetails.html": () => {
            const fairId = params.get('id');
            if (fairId && typeof window.loadFairDetailsPage === "function") {
                window.loadFairDetailsPage(fairId);
            }
        },
        "participantdetails.html": () => {
            const participantId = params.get('id');
            if (participantId && typeof window.loadParticipantDetailsPage === "function") {
                window.loadParticipantDetailsPage(participantId);
            }
        },
        "standdetails.html": () => {
            const standId = params.get('id');
            if (standId && typeof window.loadStandDetailsPage === "function") {
                window.loadStandDetailsPage(standId);
            }
        }
    };

    for (const page in pageLoaders) {
        if (path.includes(page)) {
            if (typeof pageLoaders[page] === "function") {
                await pageLoaders[page]();
            }
            break;
        }
    }
}

// SB Admin 2 Teması için Gerekli jQuery İşlevleri
// Bu blok, temanın temel etkileşimleri için zorunludur.
(function ($) {
    "use strict";

    // Olay delegasyonu ile dinamik olarak eklenen elementler için olay ataması
    $(document).on('click', '#sidebarToggle, #sidebarToggleTop', function (e) {
        $("body").toggleClass("sidebar-toggled");
        $(".sidebar").toggleClass("toggled");
        if ($(".sidebar").hasClass("toggled")) {
            $('.sidebar .collapse').collapse('hide');
        };
    });

    $(document).on('click', '.logout-trigger', function (e) {
        e.preventDefault();
        const logoutModal = new bootstrap.Modal(document.getElementById('logoutModal'));
        logoutModal.show();
    });

    $(document).on('click', '#confirmLogoutBtn', function () {
        window.logout();
    });

    // Pencere yeniden boyutlandırıldığında yan menüyü ayarla
    $(window).resize(function () {
        if ($(window).width() < 768) {
            if (!$(".sidebar").hasClass("toggled")) {
                $("body").addClass("sidebar-toggled");
                $(".sidebar").addClass("toggled");
                $('.sidebar .collapse').collapse('hide');
            };
        };

        // Masaüstü görünümde daraltılmış menüyü normale döndür
        if ($(window).width() >= 768 && $("body").hasClass("sidebar-toggled")) {
            $("body").removeClass("sidebar-toggled");
            $(".sidebar").removeClass("toggled");
        }
    });

    // Aktif sayfa scroll edildiğinde içeriğin üstten yapışmasını engelle
    $('body.fixed-nav .sidebar').on('mousewheel DOMMouseScroll wheel', function (e) {
        if ($(window).width() > 768) {
            var e0 = e.originalEvent,
                delta = e0.wheelDelta || -e0.detail;
            this.scrollTop += (delta < 0 ? 1 : -1) * 30;
            e.preventDefault();
        }
    });

    // Scroll to top butonu
    $(document).on('scroll', function () {
        var scrollDistance = $(this).scrollTop();
        if (scrollDistance > 100) {
            $('.scroll-to-top').fadeIn();
        } else {
            $('.scroll-to-top').fadeOut();
        }
    });

    // Scroll to top butonuna tıklanınca yumuşak kaydırma
    $(document).on('click', 'a.scroll-to-top', function (e) {
        var $anchor = $(this);
        $('html, body').stop().animate({
            scrollTop: ($($anchor.attr('href')).offset().top)
        }, 1000, 'easeInOutExpo'); // jquery.easing plugini gerektirir
        e.preventDefault();
    });

})(jQuery); 
