<script>
export default {
  name: 'MenuItem',
  functional: true,
  props: {
    icon: {
      type: String,
      default: ''
    },
    title: {
      type: String,
      default: ''
    }
  },
  render(h, context) {
    const { icon, title } = context.props
    const vnodes = []

    if (icon) {
      vnodes.push(<i class={[icon, 'sub-el-icon']} />)
      // if (icon.includes('el-icon')) {
      //   vnodes.push(<i class={[icon, 'sub-el-icon']} />)
      // } else {
      //   vnodes.push(<svg-icon icon-class={icon}/>)
      // }
    }

    if (title) {
      vnodes.push(<span slot='title'>{(title)}</span>)
    }
    return vnodes
  }
}
</script>

<style scoped>
.sub-el-icon {
  color: currentColor;
  width: 14px;
  height: 1em;
  font-size: 17px;
  margin-bottom: 4px;
}
</style>
