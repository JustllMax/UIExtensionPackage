using System;
using UIExtensionPackage.ExtendedUI.Enums;
using UnityEngine;
using UnityEngine.UI;


namespace UIExtensionPackage.ExtendedUI.Base
{

    /// <summary>
    /// Represents data structure, used in enhanced UI elements.
    /// </summary>
    [Serializable]
    public struct TargetGraphicData
    {
        public Selectable.Transition transition;
        public Graphic targetGraphic;
        
        public Color normalColor;
        public Color highlightedColor;
        public Color pressedColor;
        public Color selectedColor;
        public Color disabledColor;
        
        public Sprite normalSprite;
        public Sprite highlightedSprite;
        public Sprite pressedSprite;
        public Sprite selectedSprite; 
        public Sprite disabledSprite;

        public string normalTrigger;
        public string highlightedTrigger;
        public string pressedTrigger;
        public string selectedTrigger;
        public string disabledTrigger;
        
        /// <summary>
        /// Changes visuals depending on activate, interaction and transition state of TargetGraphicData.
        /// </summary>
        public void HandleVisuals(ActiveState activeState, InteractionState interactionState)
        {
            if (!targetGraphic) return;

            switch (transition)
            {
                case Selectable.Transition.ColorTint:
                    targetGraphic.color = GetColor(activeState, interactionState);
                    return;
                case Selectable.Transition.SpriteSwap:
                    SwapSprite(activeState, interactionState);
                    return;
                case Selectable.Transition.Animation:
                    SwapAnimTrigger(activeState, interactionState);
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// Changes visuals depending on activate and interaction state of TargetGraphicData.
        /// Overrides TargetGraphicData transition state
        /// </summary>
        public void HandleVisuals(ActiveState activeState, InteractionState interactionState,
            Selectable.Transition transitionState)
        {
            if (!targetGraphic) return;
            transition = transitionState;
            HandleVisuals(activeState, interactionState);
        }
        
        /// <summary>
        /// Returns color based active and interaction state
        /// </summary>
        public void SwapAnimTrigger(ActiveState activeState, InteractionState interactionState)
        {
            if(!targetGraphic) return;
            Animator animator = targetGraphic.GetComponent<Animator>();
            if(animator == null || !animator.isActiveAndEnabled || !animator.hasBoundPlayables) return;
            
            
            animator.ResetTrigger(normalTrigger);
            animator.ResetTrigger(highlightedTrigger);
            animator.ResetTrigger(pressedTrigger);
            animator.ResetTrigger(selectedTrigger);
            animator.ResetTrigger(disabledTrigger);

            animator.SetTrigger(GetTriggerName(activeState, interactionState));
           
        }

       
        /// <summary>
        /// Swaps this target graphic sprite
        /// </summary>
        //NOTE: Method cannot be type Sprite and called GetSprite(), because if the targetGraphic is not an Image,
        // it'd have to return null or default sprite, which are both bad.
        private void SwapSprite(ActiveState activeState, InteractionState interactionState)
        {
            if(targetGraphic is not Image image) return;


            if (activeState == ActiveState.Disabled)
            {
                image.sprite = disabledSprite;
                return;
            }

            switch (interactionState)
            {
                case InteractionState.Hovered:
                {
                    image.sprite = highlightedSprite;
                    return;
                }
                case InteractionState.Pressed:
                {
                    image.sprite = pressedSprite;
                    return;
                }
                case InteractionState.Selected:
                {
                    image.sprite = selectedSprite;
                    return;
                };
                default:
                {
                    image.sprite = normalSprite;
                    return;
                }
            }
        }
        
        /// <summary>
        /// Returns color based active and interaction state
        /// </summary>
        public Color GetColor(ActiveState activeState, InteractionState interactionState)
        {
            if (activeState == ActiveState.Disabled) return disabledColor;

            switch (interactionState)
            {
                case InteractionState.Hovered: return highlightedColor;
                case InteractionState.Pressed: return pressedColor;
                case InteractionState.Selected: return selectedColor;
                case InteractionState.Disabled: return disabledColor;
                default: return normalColor;
            }
        }
        
        /// <summary>
        /// Returns color based active and interaction state
        /// </summary>
        public string GetTriggerName(ActiveState activeState, InteractionState interactionState)
        {
            if (activeState == ActiveState.Disabled) return disabledTrigger;
            
            switch (interactionState)
            {
                case InteractionState.Hovered: return highlightedTrigger;
                case InteractionState.Pressed: return pressedTrigger;
                case InteractionState.Selected: return selectedTrigger;
                case InteractionState.Disabled: return disabledTrigger;
                default: return normalTrigger;
            }
        }
        
        /// <summary>
        /// Sets colors by given <see cref="ColorBlock"/>
        /// </summary>
        public void SetColors(ColorBlock colorBlock)
        {
            if(transition != Selectable.Transition.ColorTint) return;
            normalColor = colorBlock.normalColor;
            highlightedColor = colorBlock.highlightedColor;
            pressedColor = colorBlock.pressedColor;
            selectedColor = colorBlock.selectedColor;
            disabledColor = colorBlock.disabledColor;
        }
    
        /// <summary>
        /// Sets sprites by given <see cref="SpriteState"/>
        /// </summary>
        public void SetSprites(SpriteState spriteState)
        {
            if(targetGraphic is not Image) return;
            if(transition != Selectable.Transition.SpriteSwap) return;

            highlightedSprite = spriteState.highlightedSprite;
            pressedSprite = spriteState.pressedSprite;
            selectedSprite = spriteState.selectedSprite;
            disabledSprite = spriteState.disabledSprite;
        }
        
        /// <summary>
        /// Sets animation triggers by given <see cref="AnimationTriggers"/>
        /// </summary>
        public void SetAnimations(AnimationTriggers animationTriggers)
        {
            if(transition != Selectable.Transition.Animation) return;

            normalTrigger = animationTriggers.normalTrigger;
            highlightedTrigger = animationTriggers.highlightedTrigger;
            pressedTrigger = animationTriggers.pressedTrigger;
            selectedTrigger = animationTriggers.selectedTrigger;
            disabledTrigger = animationTriggers.disabledTrigger;
        }

        
        /// <summary>
        /// Set transition by given <see cref="Selectable.Transition"/>
        /// </summary>
        public void SetTransition(Selectable.Transition trans)
        {
            transition = trans;
        }


        private TargetGraphicData(Graphic graphic)
        {
            targetGraphic = graphic;
            transition = Selectable.Transition.None;
            normalColor = highlightedColor = pressedColor 
                = selectedColor = disabledColor = new Color32(255, 255, 255,255);
            
            normalTrigger =  highlightedTrigger = pressedTrigger 
                = selectedTrigger = disabledTrigger = "";
            
            normalSprite = highlightedSprite =  pressedSprite 
                = selectedSprite = disabledSprite = null;

        }

        private TargetGraphicData(Image image)
        {
            targetGraphic = image;
            transition = Selectable.Transition.None;
            normalColor = highlightedColor = pressedColor 
                = selectedColor = disabledColor = new Color32(255, 255, 255,255);
            
            normalTrigger =  highlightedTrigger = pressedTrigger 
                = selectedTrigger = disabledTrigger = "";
            
            normalSprite = highlightedSprite = pressedSprite 
                = selectedSprite = disabledSprite = image?.sprite;
        }
        
        /// <summary>
        /// Factory for creating new TargetGraphicData objects.
        /// </summary>
        public static TargetGraphicData Create(Graphic graphic)
        {
            return graphic is Image image
                ? new TargetGraphicData(image)
                : new TargetGraphicData(graphic);
        }
    }
}