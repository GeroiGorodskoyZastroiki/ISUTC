- Когда возникает ошибка про zero size, то надо удалить папку TextMesh Pro и импортировать заново
- Любые сетевые скрипты НЕ должны выключаються, а их GO НЕ должны деактивироваться
- Network for GameObjects изменён, чтобы работать с Odin и находится в папке Packages
- При компиляции иерархия материалов и материал вариантов выравнивается
- Включённая виртуализация в BIOS не даёт сделать Attach дебаггера к Unity
- При включённом превью в юнити игроки могут спавниться друг в друге - это нормальное поведение
- Camera.main не возвращает объекты, котом  после старта сцены присвоен тег MainCamera
- Destroy работает не корректно - использовать NetworkObject.Despawn()
- Для ретаргетинга нужны Humanoid анимации
- Когда происходить rebuild rig'a, то невозможно изменять transform игрока
- Изменение weight через скрипт не оказывает эффекта

-Старая интенсивность фонарика 66.8