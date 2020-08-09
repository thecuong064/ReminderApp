﻿using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Com.Ittianyu.Bottomnavigationviewex;
using Android.Support.Design.Internal;
using ReminderApp.Droid.Controls;

namespace ReminderApp.Droid.Utils
{
    using RelativeLayout = Android.Widget.RelativeLayout;
    using Platform = Platform;

    public static class CustomTabbedPageRendererUtils
    {
        public static Rectangle CreateRect(this Context context, int width, int height)
        {
            return new Rectangle(
                    0, 0,
                    context.FromPixels(width),
                    context.FromPixels(height)
                );
        }

        public static void HandlePagesChanged(this CustomTabbedPageRenderer renderer)
        {
            renderer.SetupBottomBar();
            renderer.SetupTabItems();

            if (renderer.Element.Children.Count == 0)
            {
                return;
            }

            EnsureTabIndex(renderer);
        }

        static void EnsureTabIndex(CustomTabbedPageRenderer renderer)
        {
            var rootLayout = (RelativeLayout)renderer.GetChildAt(0);
            var bottomNav = (BottomNavigationViewEx)rootLayout.GetChildAt(1);
            var menu = (BottomNavigationMenu)bottomNav.Menu;

            var itemIndex = menu.FindItemIndex(renderer.CurrentMenuItemId);
            var pageIndex = renderer.Element.Children.IndexOf(renderer.Element.CurrentPage);
            if (pageIndex >= 0 && pageIndex != itemIndex && pageIndex < bottomNav.ItemCount)
            {
                var menuItem = menu.GetItem(pageIndex);
                bottomNav.SelectedItemId = menuItem.ItemId;

                if (CustomTabbedPageRenderer.ShouldUpdateSelectedIcon && CustomTabbedPageRenderer.MenuItemIconSetter != null)
                {
                    CustomTabbedPageRenderer.MenuItemIconSetter?.Invoke(menuItem, renderer.Element.CurrentPage.Icon, true);

                    if (renderer.LastSelectedIndex != pageIndex)
                    {
                        var lastSelectedPage = renderer.Element.Children[renderer.LastSelectedIndex];
                        var lastSelectedMenuItem = menu.GetItem(renderer.LastSelectedIndex);
                        CustomTabbedPageRenderer.MenuItemIconSetter?.Invoke(lastSelectedMenuItem, lastSelectedPage.Icon, false);
                        renderer.LastSelectedIndex = pageIndex;
                    }
                }
                else if (renderer.LastSelectedIndex != pageIndex)
                {
                    renderer.LastSelectedIndex = pageIndex;
                }
            }
        }

        public static void SwitchPage(this CustomTabbedPageRenderer renderer, IMenuItem item)
        {
            var rootLayout = (RelativeLayout)renderer.GetChildAt(0);
            var bottomNav = (BottomNavigationViewEx)rootLayout.GetChildAt(1);
            var menu = (BottomNavigationMenu)bottomNav.Menu;
            renderer.CurrentMenuItemId = item.ItemId;
            var index = menu.FindItemIndex(item.ItemId);
            var pageIndex = index % renderer.Element.Children.Count;
            var currentPageIndex = renderer.Element.Children.IndexOf(renderer.Element.CurrentPage);

            if (pageIndex == currentPageIndex)
            {
                if (renderer.Element.CurrentPage is NavigationPage nav)
                {
                    nav.Navigation.PopToRootAsync();
                }
            }
            else
            {
                renderer.Element.CurrentPage = renderer.Element.Children[pageIndex];
            }
        }

        public static void Layout(this CustomTabbedPageRenderer renderer, int width, int height)
        {
            var rootLayout = (RelativeLayout)renderer.GetChildAt(0);
            var bottomNav = (BottomNavigationViewEx)rootLayout.GetChildAt(1);

            var Context = renderer.Context;

            rootLayout.Measure(
                MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly),
                MeasureSpecFactory.MakeMeasureSpec(height, MeasureSpecMode.AtMost));

            ((IPageController)renderer.Element).ContainerArea = Context.CreateRect(rootLayout.MeasuredWidth, rootLayout.GetChildAt(0).MeasuredHeight);

            rootLayout.Measure(
                MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly),
                MeasureSpecFactory.MakeMeasureSpec(height, MeasureSpecMode.Exactly));
            rootLayout.Layout(0, 0, rootLayout.MeasuredWidth, rootLayout.MeasuredHeight);

            if (renderer.Element.Children.Count == 0)
            {
                return;
            }

            int tabsHeight = bottomNav.MeasuredHeight;

            var item = (ViewGroup)bottomNav.GetChildAt(0);

            item.Measure(
                MeasureSpecFactory.MakeMeasureSpec(width, MeasureSpecMode.Exactly),
                MeasureSpecFactory.MakeMeasureSpec(tabsHeight, MeasureSpecMode.Exactly));

            item.Layout(0, 0, width, tabsHeight);

            var menuItems = bottomNav.GetBottomNavigationItemViews();
            var count = menuItems.Length;
            if (count == 0)
            {
                return;
            }
            var itemWidth = width / count;
            for (int i = 0; i < count; i++)
            {
                var menu = menuItems[i];
                menu.Measure(
                    MeasureSpecFactory.MakeMeasureSpec(itemWidth, MeasureSpecMode.Exactly),
                    MeasureSpecFactory.MakeMeasureSpec(tabsHeight, MeasureSpecMode.Exactly));
                menu.Layout(i * itemWidth, 0, itemWidth * (i + 1), tabsHeight);
            }
        }

        public static void SetupTabItems(this CustomTabbedPageRenderer renderer, BottomNavigationViewEx bottomNav)
        {
            var Element = renderer.Element;
            var menu = (BottomNavigationMenu)bottomNav.Menu;
            menu.Clear();
            var mPresenterField = Java.Lang.Class.FromType(typeof(BottomNavigationMenuView)).GetDeclaredField("presenter");
            mPresenterField.Accessible = true;
            var mPresenter = (BottomNavigationPresenter)mPresenterField.Get(bottomNav.BottomNavigationMenuView);
            mPresenterField.Accessible = false;

            if (Element.Children.Count == 0)
            {
                return;
            }

            mPresenter.SetUpdateSuspended(true);
            var tabsCount = Math.Min(Element.Children.Count, bottomNav.MaxItemCount);
            for (int i = 0; i < tabsCount; i++)
            {
                var page = Element.Children[i];
                var menuItem = menu.Add(0, i, 0, page.Title);
                var setter = CustomTabbedPageRenderer.MenuItemIconSetter ?? CustomTabbedPageRenderer.DefaultMenuItemIconSetter;
                setter.Invoke(menuItem, page.Icon, renderer.LastSelectedIndex == i);
            }
            mPresenter.SetUpdateSuspended(false);
            mPresenter.UpdateMenuView(true);

            bottomNav.EnableShiftingMode(false);//remove shifting mode
            bottomNav.EnableItemShiftingMode(false);//remove shifting mode
            bottomNav.EnableAnimation(false);//remove animation
            bottomNav.SetTextVisibility(CustomTabbedPageRenderer.VisibleTitle.HasValue ? CustomTabbedPageRenderer.VisibleTitle.Value : true);
            if (CustomTabbedPageRenderer.Typeface != null)
            {
                bottomNav.SetTypeface(CustomTabbedPageRenderer.Typeface);
            }
            if (CustomTabbedPageRenderer.IconSize.HasValue)
            {
                bottomNav.SetIconSize(CustomTabbedPageRenderer.IconSize.Value, CustomTabbedPageRenderer.IconSize.Value);
            }
            if (CustomTabbedPageRenderer.FontSize.HasValue)
            {
                bottomNav.SetTextSize(CustomTabbedPageRenderer.FontSize.Value);
            }

            bottomNav.TextAlignment = Android.Views.TextAlignment.Center;
        }

        public static BottomNavigationViewEx SetupBottomBar(this CustomTabbedPageRenderer renderer, Android.Widget.RelativeLayout rootLayout, BottomNavigationViewEx bottomNav, int barId)
        {
            if (bottomNav != null)
            {
                rootLayout.RemoveView(bottomNav);
                bottomNav.SetOnNavigationItemSelectedListener(null);
            }

            var barParams = new Android.Widget.RelativeLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                CustomTabbedPageRenderer.BottomBarHeight.HasValue ? (int)rootLayout.Context.ToPixels(CustomTabbedPageRenderer.BottomBarHeight.Value) : ViewGroup.LayoutParams.WrapContent);
            barParams.AddRule(LayoutRules.AlignParentBottom);
            bottomNav = new BottomNavigationViewEx(rootLayout.Context)
            {
                LayoutParameters = barParams,
                Id = barId
            };
            if (CustomTabbedPageRenderer.BackgroundColor.HasValue)
            {
                bottomNav.SetBackgroundColor(CustomTabbedPageRenderer.BackgroundColor.Value);
            }
            if (CustomTabbedPageRenderer.ItemIconTintList != null)
            {
                bottomNav.ItemIconTintList = CustomTabbedPageRenderer.ItemIconTintList;
            }
            if (CustomTabbedPageRenderer.ItemTextColor != null)
            {
                bottomNav.ItemTextColor = CustomTabbedPageRenderer.ItemTextColor;
            }
            if (CustomTabbedPageRenderer.ItemBackgroundResource.HasValue)
            {
                bottomNav.ItemBackgroundResource = CustomTabbedPageRenderer.ItemBackgroundResource.Value;
            }

            bottomNav.SetOnNavigationItemSelectedListener(renderer);
            rootLayout.AddView(bottomNav, 1, barParams);

            return bottomNav;
        }

        public static void ChangePage(this CustomTabbedPageRenderer renderer, FrameLayout pageContainer, Page page)
        {
            renderer.Context.HideKeyboard(renderer);

            if (page == null)
            {
                return;
            }

            if (Platform.GetRenderer(page) == null)
            {
                Platform.SetRenderer(page, Platform.CreateRendererWithContext(page, renderer.Context));
            }
            var pageContent = Platform.GetRenderer(page).View;
            pageContainer.AddView(pageContent);
            if (pageContainer.ChildCount > 1)
            {
                pageContainer.RemoveViewAt(0);
            }

            EnsureTabIndex(renderer);
        }

        public static RelativeLayout CreateRoot(this CustomTabbedPageRenderer renderer, int barId, int pageContainerId, out FrameLayout pageContainer)
        {
            var rootLayout = new RelativeLayout(renderer.Context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
            };
            var pageParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            pageParams.AddRule(LayoutRules.Above, barId);

            pageContainer = new FrameLayout(renderer.Context)
            {
                LayoutParameters = pageParams,
                Id = pageContainerId
            };

            rootLayout.AddView(pageContainer, 0, pageParams);

            return rootLayout;
        }
    }
}