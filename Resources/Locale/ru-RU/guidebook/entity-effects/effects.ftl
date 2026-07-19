# Переводы эффектов сущностей для гайдбука реагентов

entity-effect-guidebook-knockdown =
    { $type ->
        [update]
            { $chance ->
                [1] Сбивает
                *[other] сбивает
            } с ног минимум на { NATURALFIXED($time, 3) }с без накопления
        [add]
            { $chance ->
                [1] Сбивает
                *[other] сбивает
            } с ног минимум на { NATURALFIXED($time, 3) }с с накоплением
        [set]
            { $chance ->
                [1] Сбивает
                *[other] сбивает
            } с ног на { NATURALFIXED($time, 3) }с
        *[remove]
            { $chance ->
                [1] Убирает
                *[other] убирают
            } { NATURALFIXED($time, 3) }с нокдауна
    }
