# Psychological-Sanity-System-HDRP-Integrated-
Psychological Sanity System (HDRP Integrated)
Oyuncunun karanlıkta kalma, düşman tarafından kovalanma veya aranma durumlarına göre dinamik olarak azalan ve HDRP Post-Processing efektleriyle ekrana yansıyan bir akıl sağlığı yönetim modülü.

Özellikler:

Dinamik Tehlike Çarpanları: Sistem, sahnede bulunan birden fazla düşmanı (EnemyAI Array) aynı anda takip edebilir. Eğer oyuncu kovalanıyorsa (Chase) yüksek, sadece aranıyorsa (Search) orta, karanlıktaysa (Darkness) düşük seviyede bir akıl sağlığı düşüşü hesaplar.

HDRP Post-Processing Entegrasyonu: Akıl sağlığı düştükçe InsanityVolume (Chromatic Aberration, Vignette vb. içeren Volume) ağırlığı matematiksel olarak artırılır ve ekranda görsel bir anksiyete hissi yaratılır.

Event-Driven Uyarılar: Akıl sağlığı sıfırlandığında oyunu koda gömülü metodlarla kapatmak yerine On Sanity Depleted olayını tetikler. Bu sayede Cutscene (Ara sahne) veya GameOver yöneticileri kod yazılmadan sisteme bağlanabilir.

Kurulum:

Sahnede bir Game Object'e SanityManager ekleyin.

Inspector üzerinden UI Bar (Fill Image) ve HDRP Volume referanslarını verin.

Sahnedeki düşmanları Enemies listesine sürükleyin.

On Sanity Depleted olayına (event) GameOverManager.ShowGameOverScreen metodunuzu bağlayarak sistemi aktif edin.
