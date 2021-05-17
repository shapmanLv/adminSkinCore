<template>
  <div class="sidebar-logo-container" :class="{'collapse':collapse}">
    <transition name="sidebarLogoFade">
      <router-link v-if="collapse" key="collapse" class="sidebar-logo-link" to="/">
        <img v-if="logo" src="@/assets/dogfood.png" class="sidebar-logo">
        <h1 class="sidebar-title">{{ systemTitle }} </h1>
      </router-link>
      <router-link v-else key="expand" class="sidebar-logo-link" to="/">
        <img v-if="logo" src="@/assets/dogfood.png" class="sidebar-logo">
        <h1 class="sidebar-title">{{ systemTitle }} </h1>
      </router-link>
    </transition>
  </div>
</template>

<script>
import { title } from '@/settings'

export default {
  name: 'SidebarLogo',
  props: {
    collapse: {
      type: Boolean,
      required: false,
      default: false
    }
  },
  data() {
    return {
      logo: '../assets/logo.png',
      systemTitle: title
    }
  }
}
</script>

<style lang="scss" scoped>
.sidebarLogoFade-enter-active {
  transition: opacity 1.5s;
}

.sidebarLogoFade-enter,
.sidebarLogoFade-leave-to {
  opacity: 0;
}

.sidebar-logo-container {
  height: 50px;
  line-height: 50px;
  text-align: center;

  & .sidebar-logo-link {
    & .sidebar-logo {
      width: 32px;
      height: 32px;
      vertical-align: middle;
      margin-right: 12px;
      margin-left: 15px;
    }

    & .sidebar-title {
      display: inline-block;
      margin: 0;
      color: #fff;
      font-weight: 500;
      line-height: 50px;
      font-size: 14px;
      font-family: 微软雅黑;
      vertical-align: middle;
    }
  }

  &.collapse {
    .sidebar-logo {
      margin-right: 0px;
    }
  }
}
</style>
