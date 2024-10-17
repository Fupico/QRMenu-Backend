document.addEventListener("DOMContentLoaded", function() {
    // MutationObserver başlatıyoruz
    const observer = new MutationObserver(function(mutationsList, observer) {
        // Authorize butonundaki span öğesini bulup metni değiştiriyoruz
        const authorizeButton = document.querySelector('.swagger-ui .auth-wrapper .authorize span');
        if (authorizeButton && authorizeButton.innerHTML !== 'FuPiCo') {
            authorizeButton.innerHTML = 'FuPiCo';  // Metni sürekli kontrol edip değiştiriyoruz
            // console.log("Authorize button text changed to FuPiCo");
        }

        // Available authorizations başlığını değiştiriyoruz
        const authorizationPopupTitle = document.querySelector('.swagger-ui .dialog-ux .modal-ux-header h3');
        if (authorizationPopupTitle && authorizationPopupTitle.textContent.includes('Available authorizations')) {
            authorizationPopupTitle.textContent = 'FuPiCo JWT Token Center';  // Başlığı değiştiriyoruz
            // console.log("Authorization popup title changed to FuPiCo JWT Token Center");
        }
    });

    // Body elementine gözlemciyi bağlıyoruz
    observer.observe(document.body, {
        childList: true,   // DOM'a yeni elemanlar eklendiğinde tetiklenir
        subtree: true      // Tüm DOM alt ağaçlarını gözlemle
    });

    // Bearer prefixini eklemek için Authorization input alanını bul ve düzenle
    const tokenInputField = document.querySelector('input[name="authorization-value"]');
    if (tokenInputField) {
        tokenInputField.addEventListener('input', function(event) {
            const token = event.target.value;

            // Eğer token "Bearer " ile başlamıyorsa, ekleyelim
            if (token && !token.startsWith('Bearer ')) {
                event.target.value = `Bearer ${token}`;
            }
        });
    }
});
