<template>
	<view class="page">
		<view class="section-title">进阶功能</view>
		<wd-cell-group custom-class="group">
			<wd-cell
				title="菜品配置"
				label="配置菜品关联的原材料及用量"
				icon="view-list"
				is-link
				@click="go('/pages/dishes/index')"
			/>
			<wd-cell
				title="财务统计"
				label="查看收入、成本与利润分析"
				icon="chart"
				is-link
				@click="go('/pages/finance/index')"
			/>
			<wd-cell
				title="新鲜度预警"
				label="查看超过存放天数的食材"
				icon="warn-bold"
				is-link
				@click="go('/pages/freshness/index')"
			/>
		</wd-cell-group>
	</view>
</template>

<script setup>
function go(url) {
  uni.navigateTo({ url })
}
</script>

<style lang="scss">
.page {
	background: #f5f5f5;
	min-height: 100vh;
	padding-top: 16rpx;
}

.section-title {
	font-size: 24rpx;
	color: #999;
	padding: 24rpx 32rpx 8rpx;
}

:deep(.group) {
	margin-bottom: 16rpx;
}
</style>
